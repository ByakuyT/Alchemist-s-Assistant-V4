using PotionCraft.LocalizationSystem;
using PotionCraft.ObjectBased.Mortar;

namespace AlchAssV4;

public static class Calculation
{
    #region 窗口信息计算

    /// <summary>
    /// 计算研磨信息
    /// </summary>
    public static string CalculateGrind(Mortar mortar)
    {
        return mortar.ContainedStack == null
            ? ""
            : $"{LocalizationManager.GetText("研磨进度")}: {mortar.ContainedStack.overallGrindStatus * 100}%";
    }

    #endregion
}