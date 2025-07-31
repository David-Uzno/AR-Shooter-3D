Shader "Custom/OvalMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centerUV = i.uv - 0.5; // Centro del UV
                float a = 0.5; // semieje horizontal (ajustable)
                float b = 0.35; // semieje vertical (ajustable)

                float ellipse = (centerUV.x * centerUV.x) / (a * a) + (centerUV.y * centerUV.y) / (b * b);

                if (ellipse > 1.0)
                    discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}