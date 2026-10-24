using System;
using System.Collections.Generic;

namespace AlchAssV4
{
    public static class Algebra
    {
        #region 通用求根

        /// <summary>
        /// Brent算法
        /// </summary>
        public static double Brent(Func<double, double> f, double a, double b)
        {
            var fa = f(a);
            var fb = f(b);
            var fTol = 1e-12 * Math.Max(1.0, Math.Max(Math.Abs(fa), Math.Abs(fb)));
            var xTol = 1e-12 * Math.Max(1.0, Math.Max(Math.Abs(a), Math.Abs(b)));
            if (Math.Abs(fa) < fTol) return a;
            if (Math.Abs(fb) < fTol) return b;
            if (Math.Abs(fa) < Math.Abs(fb))
            {
                (a, b) = (b, a);
                (fa, fb) = (fb, fa);
            }

            var c = a;
            var fc = fa;
            var d = c;
            var mf = true;
            while (Math.Abs(fb) > fTol && Math.Abs(b - a) > xTol)
            {
                double s;
                if (fa != fc && fb != fc && fa != fb)
                    s = a * fb * fc / ((fa - fb) * (fa - fc)) + b * fa * fc / ((fb - fa) * (fb - fc)) +
                        c * fa * fb / ((fc - fa) * (fc - fb));
                else
                    s = b - fb * (b - a) / (fb - fa);
                var co1 = s < (3 * a + b) / 4 || s > b;
                var co2 = mf && Math.Abs(s - b) >= Math.Abs(b - c) / 2;
                var co3 = !mf && Math.Abs(s - b) >= Math.Abs(c - d) / 2;
                var co4 = mf && Math.Abs(b - c) < xTol;
                var co5 = !mf && Math.Abs(c - d) < xTol;
                if (co1 || co2 || co3 || co4 || co5)
                {
                    s = (a + b) / 2;
                    mf = true;
                }
                else
                    mf = false;

                var fs = f(s);
                d = c;
                c = b;
                fc = fb;
                if (Math.Sign(fa) == Math.Sign(fs))
                {
                    a = s;
                    fa = fs;
                }
                else
                {
                    b = s;
                    fb = fs;
                }

                if (Math.Abs(fa) < Math.Abs(fb))
                {
                    (a, b) = (b, a);
                    (fa, fb) = (fb, fa);
                }
            }

            return b;
        }

        /// <summary>
        /// 求解多项式在区间内的根
        /// </summary>
        public static void FindAllRoots(Variable.Poly p, double l, double r, out List<double> roots)
        {
            roots = [];
            var poly = p.NormalizedInf();
            switch (poly.Degree)
            {
                case < 1:
                    return;
                case 1:
                {
                    var x = -p.Coef(0) / p.Coef(1);
                    if (x > l + 1e-10 && x < r - 1e-10)
                        roots.Add(x);
                    return;
                }
                default:
                    FindAllRoots(poly.Derivative(), l, r, out var crits);
                    crits.Insert(0, l);
                    crits.Add(r);
                    foreach (var x in crits)
                    {
                        var y = poly.Evaluate(x);
                        if (Math.Abs(y) < 1e-10)
                            AddUniqueRoot(roots, x);
                    }

                    for (var i = 0; i < crits.Count - 1; i++)
                    {
                        var x0 = crits[i];
                        var x1 = crits[i + 1];
                        if (x1 - x0 < 1e-10) continue;
                        var y0 = poly.Evaluate(x0);
                        var y1 = poly.Evaluate(x1);
                        if (Math.Abs(y0) < 1e-10 || Math.Abs(y1) < 1e-10) continue;
                        if (Math.Sign(y0) == Math.Sign(y1)) continue;
                        var root = Brent(poly.Evaluate, x0, x1);
                        if (root > l + 1e-10 && root < r - 1e-10) AddUniqueRoot(roots, root);
                    }

                    roots.Sort();
                    break;
            }
        }

        #endregion

        #region 数值处理

        /// <summary>
        /// 添加不重复的根
        /// </summary>
        public static void AddUniqueRoot(List<double> roots, double value)
        {
            foreach (var root in roots)
                if (Math.Abs(value - root) < 1e-10)
                    return;
            roots.Add(value);
        }

        #endregion
    }
}