using Verse;

namespace UltraPerformanceMode
{
    public class UltraPerformanceSettings : ModSettings
    {
        public bool enable8Bit = true;
        public int targetResolution = 16;
        public int colorBits = 3;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enable8Bit, "enable8Bit", true);
            Scribe_Values.Look(ref targetResolution, "targetResolution", 16);
            Scribe_Values.Look(ref colorBits, "colorBits", 3);
        }

        public void DoWindowContents(UnityEngine.Rect rect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(rect);

            list.CheckboxLabeled("Enable 8-bit Mode", ref enable8Bit);

            list.Label("Target Resolution (8, 16, 32)");
            list.IntAdjuster(ref targetResolution, 1, 8);
            list.Label($"Current: {targetResolution}x{targetResolution}");

            list.Label("Color Depth (bits per channel: 3, 4, 5)");
            list.IntAdjuster(ref colorBits, 1, 1);
            list.Label($"Current: {colorBits}-bit");

            list.End();
        }
    }
}