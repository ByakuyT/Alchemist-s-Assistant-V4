using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using PotionCraft.LocalizationSystem;
using UnityEngine;

namespace AlchAssV4;

[BepInPlugin("AlchAssV4", "Alchemist's Assistant V4", "1.0.0")]
public class Main : BaseUnityPlugin
{
    #region Unity - 生命周期

    /// <summary>
    /// 游戏初始化时
    /// </summary>
    public void Awake()
    {
        Variable.WindowPositions[0] = Config.Bind("窗口位置", "路径信息", new Vector3(-12.50f, -1.85f, 0f));
        Variable.WindowPositions[1] = Config.Bind("窗口位置", "加水信息", new Vector3(-12.50f, -4.60f, 0f));
        Variable.WindowPositions[2] = Config.Bind("窗口位置", "移动信息", new Vector3(-4.10f, -4.60f, 0f));
        Variable.WindowPositions[3] = Config.Bind("窗口位置", "目标效果", new Vector3(-7.30f, -4.95f, 0f));
        Variable.WindowPositions[4] = Config.Bind("窗口位置", "酿造信息", new Vector3(-1.70f, -4.95f, 0f));
        Variable.WindowPositions[5] = Config.Bind("窗口位置", "效果偏离", new Vector3(-9.95f, -5.30f, 0f));
        Variable.WindowPositions[6] = Config.Bind("窗口位置", "活跃漩涡", new Vector3(7.65f, -4.60f, 0f));
        Variable.WindowPositions[7] = Config.Bind("窗口位置", "目标漩涡", new Vector3(10.05f, -4.60f, 0f));
        Variable.WindowPositions[8] = Config.Bind("窗口位置", "研磨信息", new Vector3(5.20f, -6.00f, 0f));
        Variable.WindowRectConfig = Config.Bind("窗口位置", "控制面板", new Rect(200, 200, 400, 400));
        Variable.ColorLines[0] = Config.Bind("颜色设置", "路径切向线", new Color(0.0f, 0.9f, 0.9f));
        Variable.ColorLines[1] = Config.Bind("颜色设置", "加水方向线", new Color(0.2f, 0.5f, 1.0f));
        Variable.ColorLines[2] = Config.Bind("颜色设置", "效果径向线", new Color(0.0f, 0.8f, 0.4f));
        Variable.ColorLines[3] = Config.Bind("颜色设置", "漩涡径向线", new Color(0.8f, 0.4f, 1.0f));
        Variable.ColorLines[4] = Config.Bind("颜色设置", "漩涡切向线", new Color(0.5f, 0.0f, 0.8f));
        Variable.ColorPaths[0] = Config.Bind("颜色设置", "路径曲线一", new Color(0.1f, 0.3f, 0.6f));
        Variable.ColorPaths[1] = Config.Bind("颜色设置", "路径曲线二", new Color(0.0f, 0.4f, 0.3f));
        Variable.ColorSpiral = Config.Bind("颜色设置", "漩涡曲线", new Color(0.7f, 0.0f, 1.0f));
        Variable.ColorRange = Config.Bind("颜色设置", "漩涡和效果范围", new Color(0.6f, 0.3f, 0.7f));
        Variable.ColorClosest = Config.Bind("颜色设置", "最近点", new Color(0.2f, 1.0f, 0.2f));
        Variable.ColorIntersection = Config.Bind("颜色设置", "交会点", new Color(1.0f, 0.8f, 0.2f));
        Variable.ColorDefeat = Config.Bind("颜色设置", "失败点", new Color(1.0f, 0.1f, 0.1f));
        Variable.ColorHandoff = Config.Bind("颜色设置", "最优交接点", new Color(1.0f, 0.6f, 0.1f));
        Variable.ColorCustomLineNormal = Config.Bind("颜色设置", "自定义方向线静态", new Color(0.3f, 0.3f, 0.3f));
        Variable.ColorCustomLineHover = Config.Bind("颜色设置", "自定义方向线选中", new Color(1.0f, 1.0f, 0.0f));
        Variable.ColorCustomMarkerNormal = Config.Bind("颜色设置", "自定义标记点静态", new Color(0.1f, 0.1f, 0.1f));
        Variable.ColorCustomMarkerHover = Config.Bind("颜色设置", "自定义标记点选中", new Color(1.0f, 0.3f, 0.3f));
        Variable.KeyWindow = Config.Bind("快捷键", "开关控制面板", new KeyboardShortcut(KeyCode.F1));
        Variable.KeyEffect = Config.Bind("快捷键", "选择目标效果", new KeyboardShortcut(KeyCode.LeftControl));
        Variable.KeyVortex = Config.Bind("快捷键", "选择目标漩涡", new KeyboardShortcut(KeyCode.LeftAlt));
        Variable.KeyCustomLine =
            Config.Bind("快捷键", "自定义方向线", new KeyboardShortcut(KeyCode.LeftControl, KeyCode.LeftShift));
        Variable.KeyCustomMarker =
            Config.Bind("快捷键", "自定义标记点", new KeyboardShortcut(KeyCode.LeftAlt, KeyCode.LeftShift));
        Variable.LineWidth = Config.Bind("其他设置", "渲染线宽", 0.075f);
        Variable.NodeSize = Config.Bind("其他设置", "渲染点大小", 0.15f);
        Variable.WindowScale = Config.Bind("其他设置", "信息窗口缩放", 0.8f);
        LocalizationManager.OnInitialize.AddListener(Localization.FormatLocalization);
        LocalizationManager.OnLocaleChanged.AddListener(Function.SetDebugWindowTitle);
        Function.LoadData();
        Harmony.CreateAndPatchAll(typeof(Main));
        Logger.LogInfo("Alchemist's Assistant V4 插件已加载");
    }

    /// <summary>
    /// 每帧更新时
    /// </summary>
    public void Update()
    {
    }

    /// <summary>
    /// GUI 更新时
    /// </summary>
    public void OnGui()
    {
    }

    #endregion
}