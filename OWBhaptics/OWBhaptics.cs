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
            ModHelper.Console.WriteLine($"My mod {nameof(OWBhaptics)} is loaded!", MessageType.Success);

            //testing bhaptics
            try
            {
                tactsuitVr = new BhapticsTactsuit();
                tactsuitVr.PlaybackHaptics("HeartBeat");
            }
            catch (Exception e) { ModHelper.Console.WriteLine(e.ToString()); }
        }
    }
}
