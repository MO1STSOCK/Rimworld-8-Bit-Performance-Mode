using Harmony;
using Verse;

namespace UltraPerformanceMode
{
    public class UltraPerformanceMod : Mod
    {
        public static UltraPerformanceSettings Settings;

        public UltraPerformanceMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<UltraPerformanceSettings>();
            HarmonyInstance.Create("UltraPerformanceMode.Patch").PatchAll();
        }

        public override string SettingsCategory() => "Ultra Performance Mode";

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }
    }
}