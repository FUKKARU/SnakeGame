using System;
using UnityEngine;

namespace NStageManager
{
    public abstract class AStageDrawer : MonoBehaviour
    {
        [SerializeField] Shader shader;

        protected StageInfo stageInfo = new StageInfo();

        private static readonly int StageInfoId = Shader.PropertyToID("_StageInfo");
        private Material material;
        private readonly float[] stageInfoAsFloatArray = new float[StageInfo.Size * StageInfo.Size]; // Shaderに受け渡す用、使い回す

        private void Awake()
        {
            material = new Material(shader);
        }

        private void OnDestroy()
        {
            if (material != null) Destroy(material);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            UpdateMaterial();
            Graphics.Blit(source, destination, material);
        }

        private void UpdateMaterial()
        {
            if (material == null) return;

            Array.Clear(stageInfoAsFloatArray, 0, stageInfoAsFloatArray.Length);

            foreach ((int x, int y) in stageInfo.Enumerate())
            {
                if (stageInfo.Get(x, y, out int value))
                {
                    // y座標を反転
                    stageInfoAsFloatArray[x + (StageInfo.Size - y - 1) * StageInfo.Size] = value;
                }
            }

            material.SetFloatArray(StageInfoId, stageInfoAsFloatArray);
        }
    }
}