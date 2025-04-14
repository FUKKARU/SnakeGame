using UnityEngine;

namespace NStageManager
{
    public static class CellType
    {
        public static readonly Color Empty = Color.white;
        public static readonly Color SnakeBody = Color.black;
        public static readonly Color RedItem = Color.red;
        public static readonly Color BlueItem = Color.blue;
        public static readonly Color GreenItem = Color.green;

        public static bool IsEmpty(this Color value) =>
            value == Empty;

        public static bool IsItem(this Color value) =>
            value == RedItem ||
            value == BlueItem ||
            value == GreenItem;
        public static Color[] GetItems() =>
            new Color[]
            {
                RedItem,
                BlueItem,
                GreenItem
            };
    }
}