using Harmony;
using UnityEngine;
using Verse;

namespace EightBitMode
{
    [StaticConstructorOnStartup]
    public static class TexturePatcher
    {
        static TexturePatcher()
            {
                var harmony = HarmonyInstance.Create("yourname.8bitperformancemode");
                harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            }
    }

    [HarmonyPatch(typeof(Texture2D), "Apply", new System.Type[] { })]
    public static class Patch_Texture2D_Apply
    {
        static void Prefix(Texture2D __instance)
        {
            try
            {
                int targetSize = 16; // Change to 8 for ultra chunky pixels

                if (__instance.width <= targetSize || __instance.height <= targetSize)
                    return;

                Texture2D scaled = new Texture2D(targetSize, targetSize, TextureFormat.RGBA32, false);

                for (int y = 0; y < targetSize; y++)
                {
                    for (int x = 0; x < targetSize; x++)
                    {
                        float u = (float)x / targetSize;
                        float v = (float)y / targetSize;
                        Color c = __instance.GetPixelBilinear(u, v);
                        scaled.SetPixel(x, y, c);
                    }
                }

                scaled.filterMode = FilterMode.Point;
                scaled.Apply();

                Graphics.CopyTexture(scaled, __instance);
            }
            catch
            {
                // Ignore errors to avoid breaking the game
            }
        }
    }
}
