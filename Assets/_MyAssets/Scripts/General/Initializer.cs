using UnityEngine;

namespace NGeneral
{
    public static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
            Screen.SetResolution(800, 800, false);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}