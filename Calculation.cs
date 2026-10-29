using HarmonyLib;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem;
using PotionCraft.ObjectBased.Mortar;
using UnityEngine;

namespace AlchAssV4;

public static class Calculation
{
    #region 窗口信息计算

    /// <summary>
    /// 计算活跃漩涡
    /// </summary>
    public static string CalculateVortex()
    {
        if (Managers.RecipeMap.CurrentVortexMapItem == null) return "";
        var colPos = Variable.Vec2D.From(Managers.RecipeMap.recipeMapObject.indicatorContainer.localPosition) +
                     Variable.Offset;
        var vortexPos = Variable.Vec2D.From(Managers.RecipeMap.CurrentVortexMapItem.thisTransform.localPosition);
        var disText = $"{(float)vortexPos.Distance(colPos)}";
        var maxText =
            $"{((CircleCollider2D)Traverse.Create(Managers.RecipeMap.CurrentVortexMapItem).Field("vortexCollider").GetValue()).radius + Variable.IndicatorRad}";
        var dirText = $"{(float)Variable.Vec2D.Right.SignedAngle(vortexPos - colPos)}";
        var tanText = Variable.LineDirections[4].HasValue ? $"{(float)Variable.LineDirections[4]}'" : "";
        var lfsText = Variable.DangerDistance[2].HasValue
            ? Localization.FormatLifeSalt(Variable.DangerDistance[2].Value)
            : "";
        return $"""
                {LocalizationManager.GetText("漩涡距离")}: {disText}
                {LocalizationManager.GetText("最大距离")}: {maxText}
                {LocalizationManager.GetText("漩涡方向")}: {dirText}
                {LocalizationManager.GetText("漩涡切向")}: {tanText}
                {LocalizationManager.GetText("加血需求")}: {lfsText}
                """;
    }

    /// <summary>
    /// 计算目标漩涡
    /// </summary>
    public static string CalculateTargetVortex()
    {
        var mapindex = Variable.MapId[Managers.RecipeMap.currentMap.potionBase.name];
        if (mapindex == 2 || Variable.VortexIndex[mapindex] < 0) return "";
        var colPos = Variable.Vec2D.From(Managers.RecipeMap.recipeMapObject.indicatorContainer.localPosition) +
                     Variable.Offset;
        var selVortex = Variable.Vortices[mapindex][Variable.VortexIndex[mapindex]];
        var disText = $"{(float)selVortex.Pos.Distance(colPos)}";
        var maxText = $"{(float)selVortex.Rad}";
        var dirText = Variable.LineDirections[3].HasValue ? $"{(float)Variable.LineDirections[3].Value}'" : "";
        var strText = Variable.ClosestPositions[1].HasValue
            ? $"{(float)selVortex.Pos.Distance(Variable.ClosestPositions[1].Value)}"
            : "";
        var ldlText = Variable.ClosestPositions[3].HasValue
            ? $"{(float)selVortex.Pos.Distance(Variable.ClosestPositions[3].Value)}"
            : "";
        return $"""
                {LocalizationManager.GetText("漩涡距离")}: {disText}
                {LocalizationManager.GetText("路径近点")}: {strText}
                {LocalizationManager.GetText("加水近点")}: {ldlText}
                {LocalizationManager.GetText("最大距离")}: {maxText}
                {LocalizationManager.GetText("漩涡方向")}: {dirText}
                """;
    }

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

    #region 渲染信息计算

    /// <summary>
    /// 更新偏移
    /// </summary>
    public static void UpdateOffset()
    {
        Variable.Offset = Variable.DisplayOffset
            ? Variable.Vec2D.From(Managers.RecipeMap.indicator.thisTransform.localPosition)
            : Variable.Vec2D.Zero;
    }

    #endregion
}