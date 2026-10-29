using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using PotionCraft.Assemblies.DataBaseSystem.PreparedObjects;
using PotionCraft.LocalizationSystem;
using UnityEngine;

namespace AlchAssV4;

public static class Localization
{
    #region 格式化文本

    /// <summary>
    /// 格式化本地化文本
    /// </summary>
    public static void FormatLocalization()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string[] tags = ["Label", "Title", "Button"];
        foreach (var tag in tags)
            RegisterLocalization($"AlchAssV4.Locs.{tag}.json", assembly);
    }

    /// <summary>
    /// 更新自定义方向线序号列宽
    /// </summary>
    public static void UpdateCustomLineLabelWidth()
    {
        var cnt = (int)Math.Ceiling(Math.Log10(Variable.CustomLineDirections.Count + 1));
        Variable.CustomLineLabelWidth = GetLabelWidth([new('0', cnt)], false);
    }

    /// <summary>
    /// 更新自定义标记点序号列宽
    /// </summary>
    public static void UpdateCustomMarkerLabelWidth()
    {
        var cnt = (int)Math.Ceiling(Math.Log10(Variable.CustomMarkerPositions.Count + 1));
        Variable.CustomMarkerLabelWidth = GetLabelWidth([new('0', cnt)], false);
    }

    /// <summary>
    /// 计算标签长度
    /// </summary>
    private static float GetLabelWidth(string[] labels, bool loc)
    {
        return labels.Select(label =>
            loc
                ? Variable.LabelStyle.CalcSize(new GUIContent(LocalizationManager.GetText(label) + "0")).x
                : Variable.LabelStyle.CalcSize(new GUIContent(label + "0")).x).Prepend(0f).Max();
    }

    #endregion

    #region 本地化

    /// <summary>
    /// 读取本地化文件
    /// </summary>
    private static List<Variable.Localization> LoadLocalization(string path, Assembly assembly)
    {
        using var reader = new StreamReader(assembly.GetManifestResourceStream(path));
        return JsonConvert.DeserializeObject<List<Variable.Localization>>(reader.ReadToEnd());
    }

    /// <summary>
    /// 注册本地化文本
    /// </summary>
    private static void RegisterLocalization(string path, Assembly assembly)
    {
        var locs = LoadLocalization(path, assembly);
        var data = Traverse.Create(typeof(LocalizationManager)).Field("localizationData").GetValue<LocalizationData>();
        foreach (var loc in locs)
        foreach (var locale in Enum.GetValues(typeof(LocalizationManager.Locale)))
            data.Add((int)locale, loc.key, loc.values.ContainsKey($"{locale}") ? loc.values[$"{locale}"] : loc.key);
    }

    #endregion
}