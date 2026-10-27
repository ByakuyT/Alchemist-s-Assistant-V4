using System;
using System.Collections.Generic;
using System.Linq;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionEffectMapItem;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit
    {
    }
}

namespace AlchAssV4
{
    public static class Variable
    {
        #region 只读数据

        public const double EffectRad = 1.53;
        public const double IndicatorRad = 0.74;

        public static readonly IReadOnlyDictionary<string, int> MapId = new Dictionary<string, int>
            { { "Water", 0 }, { "Oil", 1 }, { "Wine", 2 } };

        #endregion

        #region 功能开关

        public static bool DoPathCurve = false;

        public static bool DoVortexCurve = false;

        // 0 - 路径切向线; 1 - 加水方向线; 2 - 效果径向线; 3 - 漩涡径向线; 4 - 漩涡切向线
        public static bool[] DoLines = [false, false, false, false, false];

        #endregion

        #region 状态数据

        public static int HandoffRunning = -1;
        public static Vec2D Offset;
        public static Spiral? SpiralPhysical;

        public static PotionEffectMapItem TargetEffect;

        // 0 - 水; 1 - 油
        public static int[] VortexIndex = [-1, -1];

        // 0 - 效果； 1 - 漩涡
        public static Vec2D?[] HandoffPositions = [null, null];

        // 0 - 路径和效果; 1 - 路径和漩涡; 2 - 加水和效果; 3 - 加水和漩涡
        public static List<Vec2D>[] IntersectionPositions = [[], [], [], []];

        public static List<(Vec2D, bool)> PathPhysical = [];

        // 0 - 水; 1 - 油
        public static List<Vortex>[] Vortexs = [[], []];

        #endregion

        #region 数据结构

        /// <summary>
        /// 漩涡
        /// </summary>
        public struct Vortex
        {
            public Vec2D Pos;
            public double Rad;
        }

        /// <summary>
        /// 漩涡螺线
        /// </summary>
        public struct Spiral
        {
            public Vec2D Pos;
            public double Rot, MaxAngle, MinAngle;
            public double A => 1 / (2 * Math.PI);
        }

        /// <summary>
        /// double向量
        /// </summary>
        public readonly struct Vec2D(double x, double y)
        {
            public readonly double X = x, Y = y;
            public static Vec2D From(Vector3 v) => new(v.x, v.y);
            public static Vec2D From(Vector2 v) => new(v.x, v.y);
            public Vector3 ToVec3() => new((float)X, (float)Y, 0f);
            public Vector2 ToVec2() => new((float)X, (float)Y);
            public double SqrMagnitude => X * X + Y * Y;
            public double Magnitude => Math.Sqrt(SqrMagnitude);
            public double Dot(Vec2D v) => X * v.X + Y * v.Y;
            public double Cross(Vec2D v) => X * v.Y - Y * v.X;
            public static Vec2D operator +(Vec2D a, Vec2D b) => new(a.X + b.X, a.Y + b.Y);
            public static Vec2D operator -(Vec2D a, Vec2D b) => new(a.X - b.X, a.Y - b.Y);
            public static Vec2D operator *(Vec2D v, double k) => new(v.X * k, v.Y * k);
            public static Vec2D operator -(Vec2D v) => new(-v.X, -v.Y);
            public static Vec2D operator *(double k, Vec2D v) => v * k;
        }

        /// <summary>
        /// 多项式
        /// </summary>
        public readonly struct Poly(params double[] cofs)
        {
            private readonly double[] _cofs = cofs == null ? null : Trim(cofs);

            private static double[] Trim(double[] cofs)
            {
                var n = cofs.Length;
                while (n > 0 && cofs[n - 1] == 0.0) n--;
                if (n == 0) return null;
                var t = new double[n];
                Array.Copy(cofs, t, n);
                return t;
            }

            public int Degree => _cofs is null ? -1 : _cofs.Length - 1;
            public double Coef(int k) => k > Degree ? 0.0 : _cofs[k];

            public double Evaluate(double t)
            {
                if (Degree < 0) return 0.0;
                var r = 0.0;
                for (var i = Degree; i >= 0; i--)
                    r = r * t + _cofs[i];
                return r;
            }

            public Poly NormalizedInf()
            {
                if (Degree < 0) return default;
                var r = _cofs.Select(Math.Abs).Prepend(0.0).Max();
                return this * (1 / r);
            }

            public Poly Derivative()
            {
                if (Degree < 1) return default;
                var t = new double[Degree];
                for (var i = 1; i <= Degree; i++)
                    t[i - 1] = _cofs[i] * i;
                return new Poly(t);
            }

            public static Poly operator *(Poly a, double k)
            {
                if (a.Degree < 0 || k == 0.0) return default;
                var t = new double[a.Degree + 1];
                for (var i = 0; i <= a.Degree; i++)
                    t[i] = a._cofs[i] * k;
                return new Poly(t);
            }

            public static Poly operator *(double k, Poly a) => a * k;
            public static Poly operator -(Poly a) => a * -1.0;

            public static Poly operator +(Poly a, Poly b)
            {
                if (a.Degree < 0) return b;
                if (b.Degree < 0) return a;
                var t = new double[Math.Max(a.Degree, b.Degree) + 1];
                for (var i = 0; i < t.Length; i++)
                    t[i] = (i <= a.Degree ? a._cofs[i] : 0.0) + (i <= b.Degree ? b._cofs[i] : 0.0);
                return new Poly(t);
            }

            public static Poly operator -(Poly a, Poly b)
            {
                if (a.Degree < 0) return -b;
                if (b.Degree < 0) return a;
                var t = new double[Math.Max(a.Degree, b.Degree) + 1];
                for (var i = 0; i < t.Length; i++)
                    t[i] = (i <= a.Degree ? a._cofs[i] : 0.0) - (i <= b.Degree ? b._cofs[i] : 0.0);
                return new Poly(t);
            }

            public static Poly operator *(Poly a, Poly b)
            {
                if (a.Degree < 0 || b.Degree < 0) return default;
                var t = new double[a.Degree + b.Degree + 1];
                for (var i = 0; i <= a.Degree; i++)
                for (var j = 0; j <= b.Degree; j++)
                    t[i + j] += a._cofs[i] * b._cofs[j];
                return new Poly(t);
            }
        }

        public readonly struct Interval
        {
            public readonly double L, H;

            public Interval(double l, double h)
            {
                if (double.IsNaN(l) || double.IsNaN(h) || l > h)
                {
                    L = double.NaN;
                    H = double.NaN;
                }
                else
                {
                    L = l;
                    H = h;
                }
            }

            public bool IsEmpty => double.IsNaN(L) || double.IsNaN(H);
            public static Interval Empty => new(double.NaN, double.NaN);
            public static Interval Point(double x) => new(x, x);

            public static Interval operator -(Interval a) =>
                a.IsEmpty ? Empty : new(Algebra.Down(-a.H), Algebra.Up(-a.L));

            public static Interval operator *(Interval a, double k)
            {
                if (a.IsEmpty || double.IsNaN(k)) return Empty;
                if (k == 0.0) return new(0.0, 0.0);
                return k > 0.0
                    ? new(Algebra.Down(a.L * k), Algebra.Up(a.H * k))
                    : new(Algebra.Down(a.H * k), Algebra.Up(a.L * k));
            }

            public static Interval operator *(double k, Interval a) => a * k;

            public static Interval operator +(Interval a, Interval b)
            {
                if (a.IsEmpty || b.IsEmpty) return Empty;
                return new(Algebra.Down(a.L + b.L), Algebra.Up(a.H + b.H));
            }

            public static Interval operator -(Interval a, Interval b)
            {
                if (a.IsEmpty || b.IsEmpty) return Empty;
                return new(Algebra.Down(a.L - b.H), Algebra.Up(a.H - b.L));
            }

            public static Interval operator *(Interval a, Interval b)
            {
                if (a.IsEmpty || b.IsEmpty) return Empty;
                var l = Math.Min(Math.Min(Algebra.Down(a.L * b.L), Algebra.Down(a.L * b.H)),
                    Math.Min(Algebra.Down(a.H * b.L), Algebra.Down(a.H * b.H)));
                var h = Math.Max(Math.Max(Algebra.Up(a.L * b.L), Algebra.Up(a.L * b.H)),
                    Math.Max(Algebra.Up(a.H * b.L), Algebra.Up(a.H * b.H)));
                return new(l, h);
            }

            public Interval Sqrt()
            {
                if (IsEmpty || H <= 0.0) return Empty;
                var l = L <= 0.0 ? 0.0 : Algebra.Down(Math.Sqrt(L));
                var h = Algebra.Up(Math.Sqrt(H));
                return new(l, h);
            }

            public Interval Sin()
            {
                if (IsEmpty) return Empty;
                var l = Math.Min(Algebra.Down(Math.Sin(L)), Algebra.Down(Math.Sin(H)));
                var h = Math.Max(Algebra.Up(Math.Sin(L)), Algebra.Up(Math.Sin(H)));
                if (Contains(Math.PI / 2, 2 * Math.PI)) h = 1.0;
                if (Contains(-Math.PI / 2, 2 * Math.PI)) l = -1.0;
                return new(l, h);
            }

            public Interval Cos()
            {
                if (IsEmpty) return Empty;
                var l = Math.Min(Algebra.Down(Math.Cos(L)), Algebra.Down(Math.Cos(H)));
                var h = Math.Max(Algebra.Up(Math.Cos(L)), Algebra.Up(Math.Cos(H)));
                if (Contains(0.0, 2 * Math.PI)) h = 1.0;
                if (Contains(Math.PI, 2 * Math.PI)) l = -1.0;
                return new(l, h);
            }

            private bool Contains(double baseVal, double period)
            {
                var k = Math.Ceiling((L - baseVal) / period);
                return baseVal + k * period <= H;
            }
        }

        #endregion
    }
}