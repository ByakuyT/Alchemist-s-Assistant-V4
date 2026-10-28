using UnityEngine;

namespace AlchAssV4;

public static class Rendering
{
    #region 生成材质

    /// <summary>
    /// 生成实线材质
    /// </summary>
    private static Material CreateSolidMaterial()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return new Material(Shader.Find("Sprites/Default")) { mainTexture = texture };
    }

    /// <summary>
    /// 生成虚线材质
    /// </summary>
    private static Material CreateDashedMaterial()
    {
        var texture = new Texture2D(10, 1);
        var pixels = new Color[10];
        for (var x = 0; x < 10; x++)
            pixels[x] = x < 5 ? Color.white : Color.clear;
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Repeat;
        return new Material(Shader.Find("Sprites/Default")) { mainTexture = texture };
    }

    /// <summary>
    /// 生成矩形精灵
    /// </summary>
    private static Sprite CreateSquareSprite()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }

    /// <summary>
    /// 生成圆形精灵
    /// </summary>
    private static Sprite CreateRoundSprite()
    {
        var texture = new Texture2D(512, 512);
        var pixels = new Color[262144];
        var center = new Variable.Vec2D(255.5, 255.5);
        for (var y = 0; y < 512; y++)
        for (var x = 0; x < 512; x++)
        {
            var pos = new Variable.Vec2D(x, y);
            var dis2 = (pos - center).SqrMagnitude;
            pixels[y * 512 + x] = dis2 <= 65536 ? Color.white : Color.clear;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        var pixelsPerUnit = 256 / Variable.LineWidth.Value;
        return Sprite.Create(texture, new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    /// <summary>
    /// 生成材质和精灵
    /// </summary>
    public static void CreateMaterialAndSprites()
    {
        Variable.SolidMaterial = CreateSolidMaterial();
        Variable.DashedMaterial = CreateDashedMaterial();
        Variable.SquareSprite = CreateSquareSprite();
        Variable.RoundSprite = CreateRoundSprite();
    }

    #endregion
}