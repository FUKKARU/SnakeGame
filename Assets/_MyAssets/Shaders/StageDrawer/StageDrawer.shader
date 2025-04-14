Shader "Hidden/StageDrawer"
{
    Properties
    {
        [IntRange] _LineWidth ("LineWidth (px)", Range(1, 100)) = 10
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // ステージ情報
            // 最初の _Size * _Size 個のみを使用（ステージのサイズ変更に対応するため）
            // index = x + y * 10 にて計算している
            // 0なら白色
            // 1なら黒色
            // 2なら赤色
            // 3なら青色
            // 4なら緑色
            float _StageInfo[1000];

            float _Size;
            float _LineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.pos.xy / _ScreenParams.xy; // [0, 1]の範囲に正規化

                // 枠線を描画(Sizeが10の場合、0.1, 0.2, ... , 0.9 の位置)
                float lineWidthUV = float2(_LineWidth, _LineWidth) / _ScreenParams.xy; // UV空間でのLineWidthに変換
                float isLineX = step(frac(uv.x * _Size), lineWidthUV);
                float isLineY = step(frac(uv.y * _Size), lineWidthUV);
                float isLine = max(isLineX, isLineY);
                if (isLine == 1.0) return 0.0;

                // Infoの何番目のインデックスか計算
                float x = floor(uv.x * _Size);
                float y = floor(uv.y * _Size);
                int index = int(x + y * _Size);

                if (_StageInfo[index] == 0) return float4(1,1,1,1);
                if (_StageInfo[index] == 1) return float4(0,0,0,1);
                if (_StageInfo[index] == 2) return float4(1,0,0,1);
                if (_StageInfo[index] == 3) return float4(0,0,1,1);
                if (_StageInfo[index] == 4) return float4(0,1,0,1);
                return float4(0,0,0,1);
            }
            ENDCG
        }
    }
}
