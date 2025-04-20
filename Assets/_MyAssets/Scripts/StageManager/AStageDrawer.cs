using System;
using UnityEngine;

namespace NStageManager
{
    public abstract class AStageDrawer : MonoBehaviour
    {
        #region 派生クラスに公開するもの

        protected StageInfo stageInfo;
        protected virtual void Dispose() { }

        #endregion

        [Header("View")]
        [Space(10)]
        [SerializeField] private Shader shader;
        [SerializeField, Range(1, 100), Tooltip("ステージのサイズ")] private int size;
        [SerializeField, Range(1, 100), Tooltip("枠線の幅")] private int borderWidth;

        private static readonly int StageInfoTexId = Shader.PropertyToID("_StageInfoTex");
        private static readonly int StageSizeId = Shader.PropertyToID("_Size");
        private static readonly int BorderWidthId = Shader.PropertyToID("_BorderWidth");
        private Material material;

        private void Awake()
        {
            material = new Material(shader);
            stageInfo = new StageInfo(size);

            shader = null;
        }

        private void OnDestroy()
        {
            Dispose();

            if (material != null)
            {
                Destroy(material);
                material = null;
            }

            stageInfo = null;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            UpdateMaterial();
            Graphics.Blit(source, destination, material);
        }

        private void UpdateMaterial()
        {
            if (material == null) return;

            material.SetFloat(StageSizeId, size);
            material.SetFloat(BorderWidthId, borderWidth);

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
            material.SetTexture(StageInfoTexId, tex);
            Destroy(tex);
        }
    }
}