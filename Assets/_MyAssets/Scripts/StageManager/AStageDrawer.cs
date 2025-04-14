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

        private static readonly int StageInfoTexId = Shader.PropertyToID("_StageInfoTex");
        private static readonly int StageSizeId = Shader.PropertyToID("_Size");
        private Material theMaterial;

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

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Point
            };
            foreach ((int x, int y) in stageInfo.EnumeratePositions())
            {
                if (!stageInfo.Get(x, y, out Color value)) continue;
                tex.SetPixel(x, size - y - 1, value); // y軸は反転
            }
            tex.Apply();
            theMaterial.SetTexture(StageInfoTexId, tex);
            Destroy(tex);
        }
    }
}