using UnityEngine;

namespace Tools
{
    public static class SpriteCreator
    {
        public static Sprite CombineSprite(Texture2D background, Texture2D cover)
        {
            //Texture2D background = ;//你的Texture2D，这是叠在下面的那张
            //Texture2D cover = ;//你的Texture2D，这是叠在上面的那张
            //新tex，大小用哪个都无所谓，因为之前保证了所有素材大小一致
            Texture2D newTex = new Texture2D(background.width, background.height);

            Color[] bgColors = background.GetPixels();
            Color[] cvColors = cover.GetPixels();
            Color[] newColors = new Color[background.width * background.height];

            for (int x = 0; x < newTex.width; x++)
            for (int y = 0; y < newTex.height; y++)
            {
                int index = x + y * background.width;
                //混合背景和封面
                //注意：这个函数只适用于背景色完全不透明
                newColors[index] = NormalBlend(bgColors[index], cvColors[index]);
            }

            newTex.SetPixels(newColors);
            newTex.Apply();

            return Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
        }

        //注意：这个函数只适用于背景色完全不透明
        //如果需要考虑背景色透明的函数，请看“混合模式”的链接
        static Color NormalBlend(Color background, Color cover)
        {
            float CoverAlpha = cover.a;
            Color blendColor;
            blendColor.r = cover.r * CoverAlpha + background.r * (1 - CoverAlpha);
            blendColor.g = cover.g * CoverAlpha + background.g * (1 - CoverAlpha);
            blendColor.b = cover.b * CoverAlpha + background.b * (1 - CoverAlpha);
            blendColor.a = 1;
            return blendColor;
        }

        //你可以增加其它Blend模式，替换NormalBlend即可
    }
}