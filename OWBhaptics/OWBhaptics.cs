using System;
using OWML.Common;
using OWML.ModHelper;

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
                ModHelper.Console.WriteLine(tactsuitVr.ToString());
                tactsuitVr.PlaybackHaptics("Eating");
                ModHelper.Console.WriteLine("Eating");
            }
            catch (Exception e) { ModHelper.Console.WriteLine(e.ToString()); }
        }
    }
}
