using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PotionCraft.LocalizationSystem;

namespace AlchAssV4;

public static class Function
{
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
    public static void LoadVortexFromBin(string path, out List<Variable.Vortex> list)
    {
        var data = ReadBinaryFile($"AlchAssV4.Bins.{path}.bin");
        using var reader = new BinaryReader(new MemoryStream(data));
        var len = reader.ReadInt32();
        list = new List<Variable.Vortex>(len);
        for (var i = 0; i < len; i++)
            list.Add(new Variable.Vortex
                { Pos = new(reader.ReadDouble(), reader.ReadDouble()), Rad = reader.ReadDouble() });
    }

    #endregion
}