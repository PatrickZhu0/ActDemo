Shader "Hidden/DFM/BloomSimple" {
    Properties {
        _Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Strength ("Bloom Strength", Float) = 0.0
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off Lighting Off Fog { Mode off }
            CGPROGRAM
                #pragma vertex vert_img
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
 
                fixed4 _Color;
                sampler2D _MainTex;
                float _Strength;
 
                float4 frag (v2f_img i) : COLOR {
                    float4 col = tex2D(_MainTex, i.uv) * _Color;
                    float4 bloom = col;
                    col.rgb = bloom.rgb;
                    return col;
                }
            ENDCG
        }
    }
    Fallback off
}

