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
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        // dictionary of all feedback patterns found in the bHaptics directory
        public Dictionary<string, FileInfo> FeedbackMap = new Dictionary<string, FileInfo>();
#pragma warning disable CS0618 // remove warning that the C# library is deprecated
        public HapticPlayer hapticPlayer;
#pragma warning restore CS0618 

        public static RotationOption defaultRotationOption = new RotationOption(0.0f, 0.0f);

        #region Initializers

        public BhapticsTactsuit()
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
                catch {
                    continue;
                }

                FeedbackMap.Add(prefix, Files[i]);
            }
            systemInitialized = true;
        }
        #endregion


        #region PlayingHapticsEffects

        public void PlaybackHaptics(string key, bool forced = true, float[] rotation = null, float intensity = 1.0f, float duration = 1.0f)
        {
            if (suitDisabled) { return; }
            if (FeedbackMap.ContainsKey(key))
            {
                ScaleOption scaleOption = new ScaleOption(intensity, duration);
                RotationOption rotationActive = (rotation == null) ? defaultRotationOption : new RotationOption(rotation[0], rotation[1]);
                if (hapticPlayer.IsPlaying() && !forced)
                {
                    return;
                }
                else
                {
                    hapticPlayer.SubmitRegisteredVestRotation(key, key, rotationActive, scaleOption);
                }
            }
        }
        #endregion

        public float[] getPatternRotation(Vector3 localAcceleration)
        {
            // bhaptics starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
            // y is "up", z is "forward" in local coordinates
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            // get rid of the up/down component to analyze xz-rotation
            Vector3 flattenedHit = new Vector3(localAcceleration.x, 0f, localAcceleration.z);

            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float earlyhitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            // check if cross product points up or down, to make signed angle myself
            Vector3 earlycrossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (earlycrossProduct.y > 0f) { earlyhitAngle *= -1f; }
            // relative to player direction
            float myRotation = earlyhitAngle;
            // switch directions (bhaptics angles are in mathematically negative direction)
            myRotation *= -1f;
            // convert signed angle into [0, 360] rotation
            if (myRotation < 0f) { myRotation = 360f + myRotation; }

            // up/down shift is in y-direction
            float hitShift = localAcceleration.y;
            if (hitShift > 0) { hitShift = -0.5f; }
            else if (hitShift < 0) { hitShift = 0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5]
            else { hitShift = 0.0f; }

            return new float[] { myRotation, hitShift };
        }
    }
}