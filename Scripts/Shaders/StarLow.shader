Shader "Instanced/low star"{

    SubShader{

        Tags {
            "RenderType" = "Opaque"
        }

        Pass {

            Cull Off
            LOD 100

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma fragmentoption ARB_fragment_program_shadow

            #pragma multi_compile_instancing
            #pragma instancing_options nolightprobe nolightmap 
            #pragma shader_feature _EMISSION
            
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
            };

            uniform half4 _Color;

            v2f vert(appdata_t i) {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(i);

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.color = _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                return i.color;
            }

            ENDCG
        }
    }
}
