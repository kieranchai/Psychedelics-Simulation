//Shader "Custom/KaleidoscopeShader" {
//	Properties{
//		_MainTex("Texture", 2D) = "white" {}
//		_NumSegments("Number of Segments", Range(1, 12)) = 6
//		_RotationSpeed("Rotation Speed", Range(-10, 10)) = 1.0
//		_Intensity("Intensity", Range(0, 1)) = 1.0
//		_Colors("Colors", Color) = (1,1,1,1)
//	}
//
//		SubShader{
//			Tags { "Queue" = "Transparent" }
//
//			Pass {
//				Blend SrcAlpha OneMinusSrcAlpha // Enable transparency
//
//				CGPROGRAM
//				#pragma vertex vert
//				#pragma fragment frag
//				#include "UnityCG.cginc"
//
//				struct appdata {
//					float4 vertex : POSITION;
//					float2 uv : TEXCOORD0;
//				};
//
//				struct v2f {
//					float2 uv : TEXCOORD0;
//					float4 vertex : SV_POSITION;
//				};
//
//				sampler2D _MainTex;
//				float _NumSegments;
//				float _RotationSpeed;
//				float _Intensity;
//				fixed4 _Colors;
//
//				v2f vert(appdata v) {
//					v2f o;
//					o.vertex = UnityObjectToClipPos(v.vertex);
//					o.uv = v.uv;
//					return o;
//				}
//
//				fixed4 frag(v2f i) : SV_Target{
//					float2 uv = i.uv;
//					uv -= 0.5; // Center UV coordinates
//
//					// Calculate angle based on time and rotation speed
//					float angle = _Time.y * _RotationSpeed;
//
//					// If only one segment, directly use the angle without segment calculation
//					float segmentAngle = 2 * 3.14159 / max(_NumSegments, 1); // Ensure _NumSegments is at least 1
//
//					float theta;
//					if (_NumSegments > 1) {
//						// Calculate polar coordinates
//						float radius = length(uv);
//						theta = atan2(uv.y, uv.x);
//
//						// Map theta to segment angle
//						theta = theta - segmentAngle * floor(theta / segmentAngle);
//					}
//				 else {
//						// Directly use the angle without segment calculation
//						theta = atan2(uv.y, uv.x);
//					}
//
//					// Smooth transition between segments
//					float nextTheta = theta + segmentAngle;
//					float blendFactor = smoothstep(0.0, 0.02, abs(theta - angle)); // Adjust the smoothness by changing the second parameter
//
//					// Interpolate between current and next segment
//					float finalTheta = lerp(theta, nextTheta, blendFactor);
//
//					// Map theta back to UV space
//					float radius = length(uv);
//					uv = radius * float2(cos(finalTheta), sin(finalTheta));
//
//					uv += 0.5; // Recenter UV coordinates
//
//					// Sample the texture and apply color
//					fixed4 col = tex2D(_MainTex, uv);
//					col.rgb *= _Colors.rgb * _Intensity; // Adjust intensity and apply color
//					col.a *= _Colors.a;
//
//					return col;
//				}
//
//				ENDCG
//			}
//		}
//
//			FallBack "Diffuse"
//}


Shader "Custom/KaleidoscopeShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _NumSegments("Number of Segments", Range(1, 12)) = 6
        _RotationSpeed("Rotation Speed", Range(-10, 10)) = 1.0
        _Intensity("Intensity", Range(0, 1)) = 1.0
        _Colors("Colors", Color) = (1, 1, 1, 1)
    }

        SubShader{
            Tags { "Queue" = "Transparent" }

            Pass {
                Blend SrcAlpha OneMinusSrcAlpha // Enable transparency

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _NumSegments;
                float _RotationSpeed;
                float _Intensity;
                fixed4 _Colors;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 uv = i.uv;
                    uv -= 0.5; // Center UV coordinates

                    // Calculate angle based on time and rotation speed
                    float angle = _Time.y * _RotationSpeed;

                    // Calculate polar coordinates
                    float radius = length(uv);
                    float theta = atan2(uv.y, uv.x);

                    // If only one segment, directly use the angle without segment calculation
                    float segmentAngle = 2 * 3.14159 / max(_NumSegments, 1); // Ensure _NumSegments is at least 1

                    // Adjust theta based on number of segments
                    if (_NumSegments > 1) {
                        // Map theta to segment angle
                        theta = theta - segmentAngle * floor(theta / segmentAngle);

                        // Smooth transition between segments
                        float nextTheta = theta + segmentAngle;
                        float blendFactor = smoothstep(0.0, 0.1, abs(theta - angle)); // Adjust the smoothness as needed

                        // Interpolate between current and next segment
                        theta = lerp(theta, nextTheta, blendFactor);
                    }

                    // Rotate the entire pattern
                    theta += angle;

                    // Map theta back to UV space
                    uv = radius * float2(cos(theta), sin(theta));

                    uv += 0.5; // Recenter UV coordinates

                    // Sample the texture and apply color
                    fixed4 col = tex2D(_MainTex, uv);
                    col.rgb *= _Colors.rgb * _Intensity; // Adjust intensity and apply color
                    col.a *= _Colors.a;

                    return col;
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}
