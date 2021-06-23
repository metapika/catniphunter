Shader "CharacterShaders/ShieldOptimized"
{
    Properties
    {
        _Color("Color", COLOR) = (0,0,0,0)
        _MainTex("Hex Texture", 2D) = "white" {}
        _PulseIntensity ("Hex Pulse Intensity", float) = 3.0
        _PulseTimeScale("Hex Pulse Time Scale", float) = 2.0
        _PulsePosScale("Hex Pulse Position Scale", float) = 50.0
        _PulseTexOffsetScale("Hex Pulse Texture Offset Scale", float) = 1.5
        _HexEdgeIntensity("Hex Edge Intensity", float) = 2.0
        _HexEdgeColor("Hex Edge Color", COLOR) = (0,0,0,0)
        _HexEdgeTimeScale("Hex Edge Time Scale", float) = 2.0
        _HexEdgeWidthModifier("Hex Edge Width Modifier", Range(0,1)) = 0.8
        _HexEdgePosScale("Hex Edge Position Scale", float) = 80.0
        _EdgeIntensity("Edge Intensity", float) = 10.0
        _EdgeColor("Edge Color", COLOR) = (0,0,0,0)
        _EdgeExponent("Edge Falloff Exponent", float) = 6.0
        _IntersectIntensity("Intersection Intensity", float) = 10.0
        _IntersectExponent("Intersection Falloff Exponent", float) = 6.0
    }
	SubShader
	{
        Cull Off
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha One
        
		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexObjPos : TEXCOORD1;
                float2 screenPos : TEXCOORD2;
                float depth : TEXCOORD3;
            };

            float4 _Color;
            float _PulseIntensity;
            float _PulseTimeScale;
            float _PulsePosScale;
            float _PulseTexOffsetScale;
            float _HexEdgeIntensity;
            float4 _HexEdgeColor;
            float _HexEdgeTimeScale;
            float _HexEdgeWidthModifier;
            float _HexEdgePosScale;
            float _EdgeIntensity;
            float4 _EdgeColor;
            float _EdgeExponent;
            sampler2D _CameraDepthNormalsTexture;
            float _IntersectIntensity;
            float _IntersectExponent;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertexObjPos = v.vertex;
                o.screenPos = ComputeScreenPos(o.vertex);
                o.depth = -UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w;
                return o;
			}
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float horizontalDist = abs(i.vertexObjPos.x);
                float verticalDist = abs(i.vertexObjPos.z);

                fixed4 pulseTex = tex.g;
                fixed4 pulseTerm = pulseTex * _Color * _PulseIntensity * abs(sin(_Time.y * _PulseTimeScale - horizontalDist * _PulsePosScale + pulseTex.r * _PulseTexOffsetScale));

                fixed4 hexEdgeTex = tex.r;
                fixed4 hexEdgeTerm = hexEdgeTex * _HexEdgeColor * _HexEdgeIntensity * max(sin((horizontalDist + verticalDist) * _HexEdgePosScale - _Time.y * _HexEdgeTimeScale) - _HexEdgeWidthModifier, 0.0f) * (1 / (1 - _HexEdgeWidthModifier));
                
                fixed4 edgeTex = tex.b;
                fixed4 edgeTerm = pow(edgeTex.a, 2.7f) * _EdgeColor * _EdgeIntensity;

                float diff = tex2D(_CameraDepthNormalsTexture, i.screenPos.xy).r - i.depth;
                float intersectGradient = 1 - min(diff / _ProjectionParams.w, 1.0f);
                fixed4 intersectTerm = _Color * pow(intersectGradient, 0.1f) * _IntersectIntensity;
                
                return fixed4(_Color.rgb + pulseTerm.rgb + hexEdgeTerm.rgb + edgeTerm + intersectTerm, _Color.a);
            }

			ENDHLSL
		}
	}
}