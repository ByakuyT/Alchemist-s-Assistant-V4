using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PotionCraft.LocalizationSystem;

namespace AlchAssV4;

public static class Function
{
    #region 快捷键功能

    #endregion

    #region 调试窗口

    /// <summary>
    /// 设置调试窗口标题
    /// </summary>
    public static void SetDebugWindowTitle()
    {
        for (var i = 0; i < Variable.DebugWindows.Length; i++)
            Variable.DebugWindows[i]?.captionText.text = LocalizationManager.GetText(Variable.WindowTags[i]);
    }

    #endregion

    #region 加载数据

    /// <summary>
    /// 读取二进制文件
    /// </summary>
    private static byte[] ReadBinaryFile(string path)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    /// <summary>
    /// 加载漩涡数据
    /// </summary>
    private static void LoadVortexFromBin(string path, out List<Variable.Vortex> list)
    {
        var data = ReadBinaryFile($"AlchAssV4.Bins.{path}.bin");
        using var reader = new BinaryReader(new MemoryStream(data));
        var len = reader.ReadInt32();
        list = new List<Variable.Vortex>(len);
        for (var i = 0; i < len; i++)
            list.Add(new Variable.Vortex
                { Pos = new(reader.ReadDouble(), reader.ReadDouble()), Rad = reader.ReadDouble() });
    }

    /// <summary>
    /// 加载区域数据
    /// </summary>
    private static void LoadZoneFromBin(string path, out List<Variable.ZoneCircle> circles,
        out List<Variable.ZoneBox> boxes, out List<Variable.ZoneBvh> bvh, out int[,] occsum, out Variable.Vec2D origin)
    {
        var data = ReadBinaryFile($"AlchAssV4.Bins.{path}.bin");
        using var reader = new BinaryReader(new MemoryStream(data));
        var numCircle = reader.ReadInt32();
        var numBox = reader.ReadInt32();
        var girdX = reader.ReadInt32();
        var girdY = reader.ReadInt32();
        origin = new(reader.ReadDouble(), reader.ReadDouble());
        var numBvhNode = reader.ReadInt32();
        circles = [];
        for (var i = 0; i < numCircle; i++)
            circles.Add(new Variable.ZoneCircle
                { Pos = new(reader.ReadDouble(), reader.ReadDouble()), Rad = reader.ReadDouble() });
        boxes = [];
        for (var i = 0; i < numBox; i++)
            boxes.Add(new Variable.ZoneBox
            {
                Pos = new(reader.ReadDouble(), reader.ReadDouble()), XHalf = reader.ReadDouble(),
                YHalf = reader.ReadDouble(), Rot = reader.ReadDouble()
            });
        occsum = new int[girdX, girdY];
        for (var i = 0; i < girdX; i++)
        for (var j = 0; j < girdY; j++)
            occsum[i, j] = reader.ReadInt32();
        bvh = [];
        for (var i = 0; i < numBvhNode; i++)
        {
            var node = new Variable.ZoneBvh
            {
                XMin = reader.ReadDouble(), YMin = reader.ReadDouble(), XMax = reader.ReadDouble(),
                YMax = reader.ReadDouble(), Left = reader.ReadInt32(), Right = reader.ReadInt32()
            };
            var circleCount = reader.ReadInt32();
            node.Circles = [];
            for (var j = 0; j < circleCount; j++)
                node.Circles.Add(reader.ReadInt32());
            var boxCount = reader.ReadInt32();
            node.Boxes = [];
            for (var j = 0; j < boxCount; j++)
                node.Boxes.Add(reader.ReadInt32());
            bvh.Add(node);
        }
    }

    /// <summary>
    /// 读取数据文件
    /// </summary>
    public static void LoadData()
    {
        for (var i = 0; i < 2; i++)
            LoadVortexFromBin(Variable.VortexTags[i], out Variable.Vortices[i]);
        for (var i = 0; i < 6; i++)
            LoadZoneFromBin(Variable.ZoneTags[i], out Variable.Circles[i], out Variable.Boxes[i], out Variable.BVH[i],
                out Variable.OccSum[i], out Variable.OccOrigin[i]);
    }

    #endregion
}