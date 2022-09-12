using System;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using System.Reflection;
using HarmonyLib;

namespace OWBhaptics
{
    public class OWBhaptics : ModBehaviour
    {
        public static BhapticsTactsuit tactsuitVr;

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"Mod {nameof(OWBhaptics)} is loaded!", MessageType.Success);

            //init bhaptics player instance
            try
            {
                tactsuitVr = new BhapticsTactsuit();
                if (!tactsuitVr.suitDisabled)
                {
                    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
                }
                
            }
            catch (Exception e) { ModHelper.Console.WriteLine(e.ToString()); }
        }
    }


    /**
     * When using ship thrusters
     */
    [HarmonyPatch(typeof(ShipThrusterModel), "FireTranslationalThrusters")]
    class ShipThrustersHaptics
    {
        public static void Postfix(ShipThrusterModel __instance)
        {
        
        }
    }
}
