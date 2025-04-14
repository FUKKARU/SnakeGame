using System.Collections.Generic;
using UnityEngine;
using NGeneral;
using Random = UnityEngine.Random;

namespace NStageManager
{
    public sealed class StageInfo
    {
        private readonly int size = 0;
        private readonly Color[,] array; // 左下を(0, 0)のインデックスとする

        public StageInfo(int size)
        {
            this.size = size;
            array = new Color[size, size];
            Clear();
        }

        public int Size => size;

        public IEnumerable<(int x, int y)> EnumeratePositions()
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public bool Get(int x, int y, out Color value)
        {
            value = default;

            if (!x.InRange(0, size - 1))
                return false;

            if (!y.InRange(0, size - 1))
                return false;

            value = array[x, y];
            return true;
        }

        public bool Set(int x, int y, Color value)
        {
            if (!x.InRange(0, size - 1))
                return false;

            if (!y.InRange(0, size - 1))
                return false;

            array[x, y] = value;
            return true;
        }

        public bool Get(Vector2Int pos, out Color value) => Get(pos.x, pos.y, out value);
        public bool Get((int x, int y) pos, out Color value) => Get(pos.x, pos.y, out value);
        public bool Set(Vector2Int pos, Color value) => Set(pos.x, pos.y, value);
        public bool Set((int x, int y) pos, Color value) => Set(pos.x, pos.y, value);

        public bool IsIn(int x, int y) => x.InRange(0, size - 1) && y.InRange(0, size - 1);
        public bool IsIn(Vector2Int pos) => IsIn(pos.x, pos.y);
        public bool IsIn((int x, int y) pos) => IsIn(pos.x, pos.y);

        public (int x, int y) GetRandomPosition() => (Random.Range(0, size), Random.Range(0, size));

        public (int x, int y) GetLoopedPosition(int x, int y) => (x.Looped(size), y.Looped(size));
        public (int x, int y) GetLoopedPosition(Vector2Int pos) => GetLoopedPosition(pos.x, pos.y);
        public (int x, int y) GetLoopedPosition((int x, int y) pos) => GetLoopedPosition(pos.x, pos.y);

        public void Clear()
        {
            foreach (var pos in EnumeratePositions())
                Set(pos, CellType.Empty);
        }
    }
}