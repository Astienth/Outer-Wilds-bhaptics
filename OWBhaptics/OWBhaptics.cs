using System;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using System.Reflection;

namespace OWBhaptics
{
    public class OWBhaptics : ModBehaviour
    {
        public static BhapticsTactsuit tactsuitVr;

        private void Awake()
        {
            
        }

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"Mod {nameof(OWBhaptics)} is loaded!", MessageType.Success);

            //init bhaptics player instance
            try
            {
                tactsuitVr = new BhapticsTactsuit();
            }
            catch (Exception e) { ModHelper.Console.WriteLine(e.ToString()); }
        }

        public void Thrusters(ThrusterModel __instance, ref Vector3 ____translationalInput)
        {

        }
    }
}
