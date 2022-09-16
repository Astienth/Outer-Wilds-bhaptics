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
        public static IModHelper Helper { get; private set; }

        private void Start()
        {
            Helper = this.ModHelper;
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
    * Kill Player
    */
    [HarmonyPatch(typeof(DeathManager), "KillPlayer")]
    class OnDeath
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("Death");
            }
        }
    }

    /**
    * OnDamage
    */
    [HarmonyPatch(typeof(PlayerResources), "ApplyInstantDamage")]
    class OnDamage
    {
        public static void Postfix(ref bool __result, float damage)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled && __result)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("ImpactLonger");
            }
        }
    }

    /**
    * OnDamageShipModule
    */
    [HarmonyPatch(typeof(ShipModule), "ApplyImpact")]
    class ShipModuleOnDamage
    {
        public static void Postfix(ImpactData impact)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled && impact.speed > 15.0f)
            {
                if (OWBhaptics.FindObjectOfType<ShipCockpitController>().IsPlayerAtFlightConsole())
                {
                    OWBhaptics.tactsuitVr.PlaybackHaptics("ImpactShort");
                }
            }
        }
    }

    /**
    * OnSuitUp
    */
    [HarmonyPatch(typeof(PlayerResources), "OnSuitUp")]
    class OnSuitUp
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("SuitUp");
            }
        }
    }

    /**
    * OnRemoveSuit
    */
    [HarmonyPatch(typeof(PlayerResources), "OnRemoveSuit")]
    class OnRemoveSuit
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("SuitOff");
            }
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
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                if (__instance.GetLocalAcceleration().sqrMagnitude > 0)
                {
                    float[] rotation = OWBhaptics.tactsuitVr.getPatternRotation(__instance.GetLocalAcceleration());
                    OWBhaptics.tactsuitVr.PlaybackHaptics("Thrust", false, rotation, 0.3f);
                }
            }
        }
    }

    /**
    * When using jet thrusters
    */
    [HarmonyPatch(typeof(JetpackThrusterModel), "FireTranslationalThrusters")]
    class JetThrustersHaptics
    {
        public static void Postfix(JetpackThrusterModel __instance)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                if (__instance.GetLocalAcceleration().sqrMagnitude > 0)
                {
                    float[] rotation = OWBhaptics.tactsuitVr.getPatternRotation(__instance.GetLocalAcceleration());
                    OWBhaptics.tactsuitVr.PlaybackHaptics("JetPack", false, rotation, 0.3f);
                }
            }
        }
    }

    /**
    * OnEatMarshmallow
    */
    [HarmonyPatch(typeof(PlayerResources), "OnEatMarshmallow")]
    class OnEatMarshmallowHaptics
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("Eating");
            }
        }
    }

    /**
    * OnPressInteract
    */
    [HarmonyPatch(typeof(ShipCockpitController), "OnPressInteract")]
    class OnPressInteractHaptics
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("FlightConsoleIn");
            }
        }
    }

    /**
    * ExitFlightConsole
    */
    [HarmonyPatch(typeof(ShipCockpitController), "ExitFlightConsole")]
    class ExitFlightConsoleHaptics
    {
        public static void Postfix()
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("FlightConsoleOut");
            }
        }
    }
    
    /**
    * Checking if under sandfall
    */
    [HarmonyPatch(typeof(PlayerCharacterController), "FixedUpdate")]
    class PlayerCharacterControllerHaptics
    {
        public static void Postfix(PlayerCharacterController __instance)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled 
                && __instance._fluidDetector.InFluidType(FluidVolume.Type.SAND))
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("SandFall", false, null, 0.5f);
            }
        }
    }
        
    /**
    * When entering water
    */
    [HarmonyPatch(typeof(PlayerState), "OnCameraEnterWater")]
    class OnCameraEnterWaterHaptics
    {
        public static void Postfix(PlayerCharacterController __instance)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("WaterIn", false, null, 0.2f);
            }
        }
    }
    /**
    * When exitiing water
    */
    [HarmonyPatch(typeof(PlayerState), "OnCameraExitWater")]
    class OnCameraExitWaterHaptics
    {
        public static void Postfix(PlayerCharacterController __instance)
        {
            if (!OWBhaptics.tactsuitVr.suitDisabled)
            {
                OWBhaptics.tactsuitVr.PlaybackHaptics("WaterOut", false, null, 0.2f);
            }
        }
    }
}
