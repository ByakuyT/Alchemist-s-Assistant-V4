using System;
using System.Collections.Generic;

namespace AlchAssV4
{
    public static class Geometry
    {
        #region 计算几何

        /// <summary>
        /// 计算线段与圆的交点
        /// </summary>
        public static void LineVsCircle(Variable.Vec2D p0, Variable.Vec2D p1, Variable.Vec2D cen, double rad,
            out List<Variable.Vec2D> pos, out List<double> ti)
        {
            pos = [];
            ti = [];
            if (rad <= 0) return;
            var d = p1 - p0;
            var f = p0 - cen;
            var a = d.SqrMagnitude;
            if (a <= 0.0) return;
            var b = 2.0 * f.Dot(d);
            var c = f.SqrMagnitude - rad * rad;
            var delta = b * b - 4.0 * a * c;
            if (delta <= 0) return;
            var sqrtD = Math.Sqrt(delta);
            double[] ts = [(-b - sqrtD) / (2.0 * a), (-b + sqrtD) / (2.0 * a)];
            foreach (var t in ts)
                if (t >= 0 && t < 1)
                {
                    pos.Add(p0 + d * t);
                    ti.Add(t);
                }
        }

        #endregion
    }
}