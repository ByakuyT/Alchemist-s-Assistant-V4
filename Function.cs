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
}