using System;
using System.Collections.Generic;
using System.Linq;

namespace AlchAssV4;

public static class Geometry
{
    #region 计算几何

    /// <summary>
    /// 计算线段与圆的交点参数
    /// </summary>
    private static void LineVsCircleRoots(Variable.Vec2D p0, Variable.Vec2D p1, Variable.Vec2D cen, double rad,
        out Variable.Vec2D d, out List<double> ti)
    {
        ti = [];
        d = p1 - p0;
        if (rad <= 0) return;
        var a = d.SqrMagnitude;
        if (a <= 0.0) return;
        var f = p0 - cen;
        var b = 2.0 * f.Dot(d);
        var c = f.SqrMagnitude - rad * rad;
        var delta = b * b - 4.0 * a * c;
        if (delta < 0) return;
        var sqrtD = Math.Sqrt(delta);
        var t0 = (-b - sqrtD) / (2.0 * a);
        var t1 = (-b + sqrtD) / (2.0 * a);
        if (t0 is >= 0 and < 1) ti.Add(t0);
        if (t1 is >= 0 and < 1 && t0 != t1) ti.Add(t1);
    }

    /// <summary>
    /// 计算线段与圆的交点
    /// </summary>
    public static void LineVsCircle(Variable.Vec2D p0, Variable.Vec2D p1, Variable.Vec2D cen, double rad,
        out List<Variable.Vec2D> pos, out List<double> ti)
    {
        LineVsCircleRoots(p0, p1, cen, rad, out var d, out ti);
        pos = [.. ti.Select(t => p0 + d * t)];
    }

    /// <summary>
    /// 计算线段与圆最近的交点
    /// </summary>
    public static void LineVsCircle(Variable.Vec2D p0, Variable.Vec2D p1, Variable.Vec2D cen, double rad,
        out double? ti)
    {
        LineVsCircleRoots(p0, p1, cen, rad, out _, out var ts);
        ti = ts.Count > 0 ? ts[0] : null;
    }

    #endregion
}