using System;
using System.Collections;
using UnityEngine;
using NGeneral;
using Text = TMPro.TextMeshProUGUI;
using Random = UnityEngine.Random;

namespace NStageManager
{
    public sealed class StageManager : AStageDrawer
    {
        [Space(20)]
        [Header("Stage Settings")]
        [Space(10)]
        [SerializeField, Tooltip("ステージがループして、無限に続くかどうか")] private bool isStageInfinite;
        [SerializeField, Range(0.01f, 5.0f), Tooltip("ステップ間隔")] private float stepInterval;
        [SerializeField, Range(0, 100), Tooltip("アイテムの最大数")] private int itemMaxAmount;
        [SerializeField, Range(0, 100), Tooltip("アイテムの生成を止める、空きマスの数の境界値（＝最大値）")] private int itemGeneratingCellLimit;

        [Space(20)]
        [Header("UI")]
        [Space(10)]
        [SerializeField, Tooltip("進行方向を画面に表示するテキスト")] private Text directionText;
        [SerializeField, Tooltip("ゲームオーバーのラベル")] private Text gameEndLabel;

        // 体の座標情報. 0番目が頭. 最初は長さ3
        private Vector2Int[] bodies = { new(1, 3), new(1, 2), new(1, 1), new(1, 0) };
        private Vector2Int direction = Vector2Int.up; // 頭の進行方向（デフォルトで上）

        private bool hasChangedDirectionThisStep; // 現在のステップで、入力を受け取って進行方向が変わったかどうか

        private void Start()
        {
            stageInfo?.Clear();
            StartCoroutine(StepAndDraw());
        }

        protected override void Dispose()
        {
            directionText = null;
            gameEndLabel = null;

            bodies = null;
        }

        private void Update()
        {
            GetInputAndTurn();
            CreateItemsToView();
            UpdateDirectionText();
        }

        private IEnumerator StepAndDraw()
        {
            while (true)
            {
                if (hasChangedDirectionThisStep)
                    hasChangedDirectionThisStep = false;

                if (StepBodiesToView())
                {
                    Time.timeScale = 0;
                    if (gameEndLabel != null)
                        gameEndLabel.enabled = true;

                    yield break;
                }

                yield return new WaitForSeconds(stepInterval);
            }
        }

        /// <summary>
        /// 入力を受け取って、進行方向を更新する
        /// </summary>
        private void GetInputAndTurn()
        {
            if (hasChangedDirectionThisStep) return;

            switch (direction)
            {
                case { x: 0 }:
                    {
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            direction = Vector2Int.left;
                            hasChangedDirectionThisStep = true;
                            return;
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            direction = Vector2Int.right;
                            hasChangedDirectionThisStep = true;
                            return;
                        }
                    }
                    break;
                case { y: 0 }:
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            direction = Vector2Int.up;
                            hasChangedDirectionThisStep = true;
                            return;
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            direction = Vector2Int.down;
                            hasChangedDirectionThisStep = true;
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 進行方向を、UIのテキストに表示する
        /// </summary>
        private void UpdateDirectionText()
        {
            if (directionText == null) return;

            directionText.text = direction switch
            {
                { x: 0, y: 1 } => "↑",
                { x: 0, y: -1 } => "↓",
                { x: 1, y: 0 } => "→",
                { x: -1, y: 0 } => "←",
                _ => "None"
            };
        }

        /// <summary>
        /// 進行方向の情報を元に、体の座標情報を更新
        /// 体をステージに表示する
        /// </summary>
        /// <returns>頭が死亡する場所に来たら、trueを返す. そうでないなら、falseを返す</returns>
        private bool StepBodiesToView()
        {
            // 尾の座標を保存
            Vector2Int preTail = bodies[^1];

            // 体の座標情報を更新
            for (int i = bodies.Length - 1; i > 0; i--)
                bodies[i] = bodies[i - 1];

            // 頭の座標情報を更新
            // この時、アイテムを取る（＝体で上書きする）ことがある
            Vector2Int nextHead = bodies[0] + direction; // ここにアイテムがあったら、体で上書きされるので、アイテムを取ったということ
            bool gotItem = stageInfo.Get(nextHead, out Color value) && value.IsItem();
            bodies[0] = nextHead;
            if (isStageInfinite)
                bodies[0] = stageInfo.GetLoopedPosition(bodies[0]).ToVector2Int();

            // ステージに表示する
            // 尾だったマスは、空きマスに戻す
            foreach (Vector2Int body in bodies)
                stageInfo.Set(body, CellType.SnakeBody);
            stageInfo.Set(preTail, CellType.Empty);


            // 頭が体とぶつかったら死亡
            for (int i = 1; i < bodies.Length; i++)
            {
                if (bodies[0] == bodies[i])
                    return true;
            }

            // ステージ外に出たら死亡
            if (!isStageInfinite && !stageInfo.IsIn(bodies[0]))
                return true;

            // アイテムを取ったら、体を伸ばす
            // 尾だったマスを体にするので、マスが意図せず上書きされることはない
            if (gotItem)
            {
                Array.Resize(ref bodies, bodies.Length + 1);
                bodies[^1] = preTail;
                stageInfo.Set(preTail, CellType.SnakeBody);
            }

            return false;
        }

        /// <summary>
        /// フィールドに、ランダムにアイテムを生成する（上限個数は超えない）
        /// フィールドの空きマスの数が少なすぎたら、そもそも生成しない
        /// アイテムをステージに表示する
        /// </summary>
        private void CreateItemsToView()
        {
            // 空きマスの数を数え、アイテム生成の上限を超えたら、何もしない
            int emptyCount = 0;
            foreach (var pos in stageInfo.EnumeratePositions())
            {
                if (stageInfo.Get(pos, out Color value) && value.IsEmpty())
                    emptyCount++;
            }
            if (emptyCount <= itemGeneratingCellLimit) return;

            // フィールド上のアイテムの数を数え、上限を超えたら、何もしない
            int itemCount = 0;
            foreach (var pos in stageInfo.EnumeratePositions())
            {
                if (stageInfo.Get(pos, out Color value) && value.IsItem())
                    itemCount++;
            }
            if (itemCount >= itemMaxAmount) return;

            // 上限値に足りない分だけ、アイテムを生成する
            int createAmount = itemMaxAmount - itemCount;
            for (int _ = 0; _ < createAmount; _++)
            {
                // 空きマスをランダムに選ぶ
                var pos = stageInfo.GetRandomPosition();

                // 空きマスでなければ、やり直し
                if (!(stageInfo.Get(pos, out Color value) && value == CellType.Empty)) continue;

                // 種類をランダムに決定
                Color[] items = CellType.GetItems();
                Color item = items[Random.Range(0, items.Length)];

                // アイテムを生成する
                stageInfo.Set(pos, item);
            }
        }
    }
}