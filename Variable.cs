using System;
using System.Collections.Generic;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.PotionEffectMapItem;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.VortexMapItem;
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

        public static readonly Dictionary<string, int> MapId = new() { { "Water", 0 }, { "Oil", 1 }, { "Wine", 2 } };

        #endregion

        #region 功能开关

        public static bool DoPathCurve = false;

        // 0 - 路径切向线; 1 - 加水方向线; 2 - 效果径向线; 3 - 漩涡径向线; 4 - 漩涡切向线
        public static bool[] DoLines = [false, false, false, false, false];

        #endregion

        #region 状态数据

        public static int HandoffRunning = -1;
        public static Vector3 Offset;

        public static PotionEffectMapItem TargetEffect;

        // 0 - 水; 1 - 油
        public static int[] VortexIndex = [-1, -1];

        // 0 - 效果； 1 - 漩涡
        public static Vector2?[] HandoffPositions = [null, null];

        // 0 - 路径和效果; 1 - 路径和漩涡; 2 - 加水和效果; 3 - 加水和漩涡
        public static List<Vector2>[] IntersectionPositions = [[], [], [], []];

        public static List<(Vector3, bool)> PathPhysical = [];

        // 0 - 水; 1 - 油
        public static List<Vortex>[] Vortexs = [[], []];

        #endregion

        #region 数据结构

        /// <summary>
        /// 漩涡
        /// </summary>
        public struct Vortex
        {
            public Vector3 Pos;
            public double Rad;
        }

        /// <summary>
        /// double向量
        /// </summary>
        public readonly record struct Vec2D(double X, double Y)
        {
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
        public readonly record struct Poly
        {
            private readonly double[] _cofs;

            public Poly(params double[] cofs)
            {
                _cofs = cofs == null ? null : Trim(cofs);
            }

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
                var r = 0.0;
                foreach (var cof in _cofs)
                    r = Math.Max(r, Math.Abs(cof));
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

        #endregion
    }
}