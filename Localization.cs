using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using PotionCraft.Assemblies.DataBaseSystem.PreparedObjects;
using PotionCraft.LocalizationSystem;

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
            if (loc.values.ContainsKey($"{locale}"))
                data.Add((int)locale, loc.key, loc.values[$"{locale}"]);
            else
                data.Add((int)locale, loc.key, loc.key);
    }

    #endregion
}