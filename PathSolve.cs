using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx;
using PotionCraft.ManagersSystem;
using UnityEngine;

namespace AlchAssV4
{
    public static class PathSolve
    {
        #region 计算核心

        /// <summary>
        /// 计算路径 → 加水
        /// </summary>
        public static Vector2? PathToLadle(List<(Vector3, bool)> path, Vector3 offset, Vector3 cen, double rad)
        {
            if (path.Count < 2) return null;
            var O = Variable.Vec2D.From(offset);
            var C = Variable.Vec2D.From(cen);
            var gate = new object();
            var bestP = (double.PositiveInfinity, -1, 0.0);
            Parallel.For(0, path.Count, () => (double.PositiveInfinity, -1, 0.0),
                (i, _, local) =>
                {
                    var B = Variable.Vec2D.From(path[i].Item1);
                    Geometry.LineVsCircle(B, O, C, rad, out var _, out var ti);
                    var cur = (ti.Count > 0 ? ti[0] * (O - B).Magnitude : double.PositiveInfinity, i, 0.0);
                    local = Better(cur, local) ? cur : local;
                    if (i == path.Count - 1 || path[i + 1].Item2) return local;
                    var D = Variable.Vec2D.From(path[i + 1].Item1) - B;
                    var U = B - C;
                    var W = O - B;
                    var d2 = D.SqrMagnitude;
                    var u2 = U.SqrMagnitude;
                    var w2 = W.SqrMagnitude;
                    var ud = U.Dot(D);
                    var wd = W.Dot(D);
                    var uw = U.Dot(W);
                    var a = new Variable.Poly(w2, -2.0 * wd, d2);
                    var b = new Variable.Poly(uw, wd - ud, -d2);
                    var c = new Variable.Poly(u2 - rad * rad, 2.0 * ud, d2);
                    var e = new Variable.Poly(ud, d2);
                    var v = new Variable.Poly(wd, -d2);
                    var p = a * e;
                    var k = v * (a + b) - p;
                    var r = a * p * p - 2.0 * b * p * k + c * k * k;
                    Algebra.FindAllRoots(r, 0.0, 1.0, out var rs);
                    foreach (var ri in rs)
                    {
                        double t;
                        var ar = a.Evaluate(ri);
                        var br = b.Evaluate(ri);
                        var cr = c.Evaluate(ri);
                        var pr = p.Evaluate(ri);
                        var kr = k.Evaluate(ri);
                        if (Math.Abs(kr) > 1e-10)
                        {
                            t = -pr / kr;
                            var t2 = -2.0 * br / ar - t;
                            if (t < 0.0 || t > t2 || t2 > 1.0) continue;
                        }
                        else
                        {
                            var delta = br * br - ar * cr;
                            if (delta < 0.0) continue;
                            t = (-br - Math.Sqrt(delta)) / ar;
                            if (t is < 0.0 or > 1.0) continue;
                            var hs = Math.Abs(pr) + Math.Abs(kr * t);
                            if (Math.Abs(pr + kr * t) > 1e-10 * hs) continue;
                        }

                        var cu2 = (t * Math.Sqrt(ar), i, ri);
                        local = Better(cu2, local) ? cu2 : local;
                    }

                    return local;
                },
                local =>
                {
                    lock (gate)
                        if (Better(local, bestP))
                            bestP = local;
                });
            if (bestP.Item2 < 0) return null;
            if (bestP.Item2 == path.Count - 1)
                return path[bestP.Item2].Item1;
            var B = Variable.Vec2D.From(path[bestP.Item2].Item1);
            var D = Variable.Vec2D.From(path[bestP.Item2 + 1].Item1) - B;
            return (B + D * bestP.Item3).ToVec2();
        }

        #endregion

        #region 计算入口

        /// <summary>
        /// 路径 → 加水计算入口
        /// </summary>
        public static void StartPathToLadle()
        {
            ClearHandoff();
            if (!Variable.DoPathCurve || !Variable.DoLines[1]) return;
            var doEffect = Variable.TargetEffect != null && Variable.IntersectionPositions[0].Count == 0;
            var mapId = Variable.MapId[Managers.RecipeMap.currentMap.potionBase.name];
            var vortexId = mapId < 2 ? Variable.VortexIndex[mapId] : -1;
            var doVortex = vortexId >= 0 && Variable.IntersectionPositions[1].Count == 0;
            var effectPos = doEffect ? Variable.TargetEffect.transform.localPosition : default;
            var selVortex = doVortex ? Variable.Vortexs[mapId][vortexId] : default;
            var path = new List<(Vector3, bool)>(Variable.PathPhysical);
            var offset = Variable.Offset;
            ThreadingHelper.Instance.StartAsyncInvoke(() =>
            {
                var effectHandoff = doEffect ? PathToLadle(path, offset, effectPos, 1.53) : null;
                var vortexHandoff = doVortex ? PathToLadle(path, offset, selVortex.Pos, selVortex.Rad) : null;
                return () =>
                {
                    if (effectHandoff.HasValue)
                        Variable.HandoffPositions[0] = effectHandoff.Value;
                    if (vortexHandoff.HasValue)
                        Variable.HandoffPositions[1] = vortexHandoff.Value;
                };
            });
        }

        /// <summary>
        /// 漩涡 → 加水计算入口
        /// </summary>
        public static void StartVortexToLadle()
        {
        }

        /// <summary>
        /// 加水 → 路径计算入口
        /// </summary>
        public static void StartLadleToPath()
        {
        }

        /// <summary>
        /// 漩涡 → 路径计算入口
        /// </summary>
        public static void StartVortexToPath()
        {
        }

        /// <summary>
        /// 清除计算结果
        /// </summary>
        public static void ClearHandoff()
        {
            Variable.HandoffRunning = -1;
            Variable.HandoffPositions = [null, null];
        }

        #endregion

        #region 辅助计算

        public static bool Better((double, int, double) a, (double, int, double) b)
        {
            if (Math.Abs(a.Item1 - b.Item1) > 1e-10) return a.Item1 < b.Item1;
            if (a.Item2 != b.Item2) return a.Item2 < b.Item2;
            return a.Item3 < b.Item3;
        }

        #endregion
    }
}