using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace UltraPerformanceMode
{
    [HarmonyPatch(typeof(GraphicDatabase), "Get")]
    public static class Patch_GraphicDatabase_Get
    {
        static void Postfix(ref Graphic __result)
        {
            var settings = UltraPerformanceMod.Settings;
            if (!settings.enable8Bit) return;
            if (__result == null) return;

            // Multi-texture graphics
            if (__result is Graphic_Multi gm)
            {
                // Create a new Graphic_Multi with downsampled materials
                try
                {
                    Material[] mats = new Material[4];
                    mats[0] = Make8Bit(gm.MatNorth);
                    mats[1] = Make8Bit(gm.MatSouth);
                    mats[2] = Make8Bit(gm.MatEast);
                    mats[3] = Make8Bit(gm.MatSingle);
                    
                    // We can't directly create a new Graphic_Multi, so we modify the materials in place
                    // by using reflection to access the private fields
                    var field = typeof(Graphic_Multi).GetField("matsNorth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null) field.SetValue(__result, new Material[] { mats[0] });
                }
                catch { }
                return;
            }

            // For single graphics, modify the texture
            if (__result.MatSingle != null)
            {
                Texture2D tex = __result.MatSingle.mainTexture as Texture2D;
                if (tex != null)
                {
                    Texture2D newTex = DownsampleTo8Bit(tex);
                    Material newMat = new Material(__result.MatSingle);
                    newMat.mainTexture = newTex;
                    
                    // Try to update the material using reflection
                    var matField = typeof(Graphic).GetField("matSingle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (matField != null) matField.SetValue(__result, newMat);
                }
            }
        }

        private static Material Make8Bit(Material original)
        {
            if (original == null) return original;

            Texture2D tex = original.mainTexture as Texture2D;
            if (tex == null) return original;

            Texture2D newTex = DownsampleTo8Bit(tex);

            Material mat = new Material(original);
            mat.mainTexture = newTex;
            return mat;
        }

        private static Texture2D DownsampleTo8Bit(Texture2D source)
        {
            int size = UltraPerformanceMod.Settings.targetResolution;

            RenderTexture rt = RenderTexture.GetTemporary(size, size);
            Graphics.Blit(source, rt);

            Texture2D small = new Texture2D(size, size, TextureFormat.RGBA32, false);
            RenderTexture.active = rt;
            small.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            small.Apply();
            RenderTexture.ReleaseTemporary(rt);

            Color32[] pixels = small.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = QuantizeColor(pixels[i]);

            small.SetPixels32(pixels);
            small.Apply();

            return small;
        }

        private static Color32 QuantizeColor(Color32 c)
        {
            int bits = UltraPerformanceMod.Settings.colorBits;
            int shift = 8 - bits;

            byte r = (byte)((c.r >> shift) << shift);
            byte g = (byte)((c.g >> shift) << shift);
            byte b = (byte)((c.b >> shift) << shift);

            return new Color32(r, g, b, c.a);
        }
    }
}