using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bhaptics.Tact;
using UnityEngine;

namespace OWBhaptics
{

    public class BhapticsTactsuit : MonoBehaviour
    {
        public static bool suitDisabled = true;
        public static bool systemInitialized = false;
        // dictionary of all feedback patterns found in the bHaptics directory
        public static Dictionary<string, FileInfo> FeedbackMap = new Dictionary<string, FileInfo>();
#pragma warning disable CS0618 // remove warning that the C# library is deprecated
        public static HapticPlayer hapticPlayer;
#pragma warning restore CS0618 

        public static RotationOption defaultRotationOption = new RotationOption(0.0f, 0.0f);

        #region Initializers

        public void Awake()
        {
            try
            {
#pragma warning disable CS0618 // remove warning that the C# library is deprecated
                hapticPlayer = new HapticPlayer("OuterWilds_bhaptics", "OuterWilds_bhaptics");
#pragma warning restore CS0618
                suitDisabled = false;
            }
            catch
            {
                return;
            }
            RegisterAllTactFiles();
            PlaybackHaptics("HeartBeat");
        }

        /**
         * Registers all tact files in bHaptics folder
         */
        void RegisterAllTactFiles()
        {
            if (suitDisabled) { return; }
            // Get location of the compiled assembly and search through "bHaptics" directory and contained patterns
            string assemblyFile = Assembly.GetExecutingAssembly().Location;
            string myPath = Path.GetDirectoryName(assemblyFile);
            string configPath = myPath + "\\bHaptics";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.tact", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    hapticPlayer.RegisterTactFileStr(prefix, tactFileStr);
                }
                catch (Exception e) {
                    //ModHelp.Console.WriteLine(e.ToString()); 
                    throw new Exception(e.ToString());
                }

                FeedbackMap.Add(prefix, Files[i]);
            }
            systemInitialized = true;
        }
        #endregion


        #region PlayingHapticsEffects

        public void PlaybackHaptics(string key, float intensity = 1.0f, float duration = 1.0f)
        {
            if (suitDisabled) { return; }
            if (FeedbackMap.ContainsKey(key))
            {
                ScaleOption scaleOption = new ScaleOption(intensity, duration);
                hapticPlayer.SubmitRegisteredVestRotation(key, key, defaultRotationOption, scaleOption);
            }
            else
            {
                //ModHelp.Console.WriteLine("Feedback not registered: " + key);
                throw new Exception("Feedback not registered: " + key);
            }
        }
        #endregion
    }
}