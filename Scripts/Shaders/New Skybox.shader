Shader "Custom/Color Skybox" {

Properties {
    _SkyTint ("Sky", Color) = (.5, .5, .5, 1)
    _HorizonColor ("Horizon", Color) = (1.0, 1.0, 1.0, 1)
    _GroundColor  ("Ground", Color) = (.369, .349, .341, 1)
    _HorizonSize  ("HorizonSize",float) = 0.55
    _SunAngle     ("Sun Angle", float) = 0.5
    _SunSize      ("Sun Size", float) = 0.05
    [HDR]         
    _SunColor     ("Sun Color", Color) = (0, .8, 0, 1)
    _unit         ("Unit", float) = 5
    _res          ("res", float) = 100
    _sunXPosition ("sunXPosition", float) = .2
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest

        uniform half3 _SkyTint;
        uniform half3 _HorizonColor;
        uniform half3 _GroundColor;
        uniform float3 _SunColor;
        uniform float _SunSize;
        uniform float _SunAngle;
        uniform float _unit;
        uniform float _res;
        uniform float _sunXPosition;

        uniform half _HorizonSize = 0.55;

        static half3 col;
        static half3 eyeRay;

        struct appdata_t
        {
            half4 vertex : POSITION;
        };

        struct v2f
        {
            half4  pos              : SV_POSITION;
            half3  skyPosition      : TEXCOORD0; // normalized            
            half3  fragPosition     : TEXCOORD1;
        };

        float Distance(float3 a, float3 b)
        {
            const float diff_x = a.x - b.x;
            const float diff_y = a.y - b.y;
            const float diff_z = a.z - b.z;
            return sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        v2f vert (appdata_t v)
        {
            v2f OUT;
            OUT.pos = UnityObjectToClipPos(v.vertex);

            eyeRay = normalize(mul((half3x3)unity_ObjectToWorld, v.vertex.xyz));
            
            OUT.skyPosition  = sign(eyeRay) * pow(abs(eyeRay), _HorizonSize);
            OUT.fragPosition = sign(eyeRay) * pow(abs(eyeRay), 1);
            return OUT;
        }

        half4 frag (v2f IN) : SV_Target
        {
            col = half3(0.0, 0.0, 0.0);

            if (IN.skyPosition.y < 0)  {
                col = lerp(_HorizonColor, _GroundColor, saturate((1-IN.skyPosition.y-1) / _HorizonSize));
            }
            else {
                col = lerp(_HorizonColor, _SkyTint, saturate((IN.skyPosition.y) / _HorizonSize));
            }
            
            const half3 sunPos = half3(0, sin(_SunAngle), cos(_SunAngle));

            const float DistToSun = Distance(IN.fragPosition, sunPos); // returns 0-1

            if (DistToSun < _SunSize)
            {
                col = _SunColor;
            }
            {
                //else if (DistToSun < _SunSize * 2.5) // we are creating sun glow 3 times bigger than sun and we want it starts unit white and clear color transition
                //{
                //  float dist = (DistToSun - _SunSize) * .65;
                //  const float percent = ((1 - (dist / (_SunSize * 2.5))) / 2) * (IN.skyPosition.y * IN.skyPosition.y);
                //
                //  col += half3(percent,percent,percent) * (_SunColor / 2);
                //}
                //else if (DistToSun < _SunSize * 6) // 2.nd pass for holo 
                //{
                //  float dist = (DistToSun - _SunSize) * .9;
                //  //dist = dist * dist;
                //  const float percent = ((1 - (dist / (_SunSize * 6))) / 4) * (IN.skyPosition.y * IN.skyPosition.y);
                //
                //  col += half3(percent,percent,percent) * (_SunColor / 2);
                //}
            }
            return half4(col, 1.0);
        }
        ENDCG
    }
}
Fallback Off
}