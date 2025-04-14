using UnityEngine;

namespace NGeneral
{
    public static class Extensions
    {
        /// <summary>
        /// valueが区間[min, max]に含まれるかどうかを判定する
        /// </summary>
        public static bool InRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// valueが区間[min, max]に含まれるかどうかを判定する
        /// </summary>
        public static bool InRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// valueが[0, length)の範囲に収まるように、ループさせる
        /// </summary>
        public static int Looped(this int value, int length)
        {
            while (value < 0)
                value += length;

            while (value >= length)
                value -= length;

            return value;
        }

        /// <summary>
        /// valueが[0, length)の範囲に収まるように、ループさせる
        /// </summary>
        public static float Looped(this float value, float length)
        {
            while (value < 0)
                value += length;

            while (value >= length)
                value -= length;

            return value;
        }

        /// <summary>
        /// タプルをVector2Intに変換する
        /// </summary>
        public static Vector2Int ToVector2Int(this (int x, int y) value) => new Vector2Int(value.x, value.y);

        /// <summary>
        /// Vector2Intをタプルに変換する
        /// </summary>
        public static (int x, int y) ToTuple(this Vector2Int value) => (value.x, value.y);
    }
}