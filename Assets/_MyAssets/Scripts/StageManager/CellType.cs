namespace NStageManager
{
    public static class CellType
    {
        public static readonly int Empty = 0;
        public static readonly int SnakeBody = 1;
        public static readonly int RedItem = 2;
        public static readonly int BlueItem = 3;
        public static readonly int GreenItem = 4;

        public static bool IsEmpty(this int value) =>
            value == Empty;

        public static bool IsItem(this int value) =>
            value == RedItem ||
            value == BlueItem ||
            value == GreenItem;
        public static int[] GetItems() =>
            new int[]
            {
                RedItem,
                BlueItem,
                GreenItem
            };
    }
}