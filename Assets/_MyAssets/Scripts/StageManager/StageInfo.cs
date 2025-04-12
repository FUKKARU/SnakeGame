using System.Collections.Generic;
using UnityEngine;
using NGeneral;

namespace NStageManager
{
    public sealed class StageInfo
    {
        public static readonly int Size = 10;

        private int[,] Array = new int[Size, Size]; // trueなら塗る、falseなら塗らない. 左下を(0, 0)のインデックスとする

        public IEnumerable<(int x, int y)> Enumerate()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public bool Get(int x, int y, out int value)
        {
            value = 0;

            if (!x.InRange(0, Size - 1))
                return false;

            if (!y.InRange(0, Size - 1))
                return false;

            value = Array[x, y];
            return true;
        }

        public bool Set(int x, int y, int value)
        {
            if (!x.InRange(0, Size - 1))
                return false;

            if (!y.InRange(0, Size - 1))
                return false;

            Array[x, y] = value;
            return true;
        }

        public bool Get(Vector2Int pos, out int value) => Get(pos.x, pos.y, out value);
        public bool Set(Vector2Int pos, int value) => Set(pos.x, pos.y, value);

        public void Clear() => Array = new int[Size, Size];
    }
}