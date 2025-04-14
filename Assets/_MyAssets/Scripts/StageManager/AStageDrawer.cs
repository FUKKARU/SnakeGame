using System;
using UnityEngine;

namespace NStageManager
{
    public abstract class AStageDrawer : MonoBehaviour
    {
        #region 派生クラスに公開するもの

        protected StageInfo stageInfo;

        #endregion


        [Header("View")]
        [Space(10)]
        [SerializeField] private Material material;
        [SerializeField, Range(1, 30), Tooltip("ステージのサイズ")] private int size;

        private static readonly int StageInfoId = Shader.PropertyToID("_StageInfo");
        private static readonly int StageSizeId = Shader.PropertyToID("_Size");
        private Material theMaterial;

        // Shaderに受け渡す用、使い回す
        // 最初の size * size 個のみを使用（ステージのサイズ変更に対応するため）
        private readonly float[] stageInfoAsFloatArray = new float[1000];

        private void Awake()
        {
            theMaterial = new Material(material);
            stageInfo = new StageInfo(size);
        }

        private void OnDestroy()
        {
            if (theMaterial != null) Destroy(theMaterial);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            UpdateMaterial();
            Graphics.Blit(source, destination, theMaterial);
        }

        private void UpdateMaterial()
        {
            if (theMaterial == null) return;

            theMaterial.SetFloat(StageSizeId, size);

            Array.Clear(stageInfoAsFloatArray, 0, stageInfoAsFloatArray.Length);
            foreach ((int x, int y) in stageInfo.EnumeratePositions())
            {
                if (!stageInfo.Get(x, y, out int value)) continue;
                int index = x + (size - y - 1) * size; // y座標を反転
                stageInfoAsFloatArray[index] = value;
            }
            theMaterial.SetFloatArray(StageInfoId, stageInfoAsFloatArray);
        }
    }
}