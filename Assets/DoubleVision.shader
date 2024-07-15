Shader "AdjustableBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DistortionFrequency("Distortion Frequency", float) = 1.0
        _BlurIntensity("Blur Intensity", float) = 0.01
    }
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vertex_shader
                #pragma fragment pixel_shader
                #pragma target 2.0

                sampler2D _MainTex;
                float _DistortionFrequency;
                float _BlurIntensity;

                float4 vertex_shader(float4 vertex : POSITION) : SV_POSITION
                {
                    return UnityObjectToClipPos(vertex);
                }

                float4 pixel_shader(float4 vertex : SV_POSITION) : COLOR
                {
                    // Calculate UV coordinates
                    float2 uv = vertex.xy / _ScreenParams.xy;
                    //uv.y = 1.0 - uv.y;

                    // Apply distortion effect
                    float timeFactor = _Time.g * _DistortionFrequency; // Adjust frequency of effect
                    float offset = sin(timeFactor) * _BlurIntensity; // Adjust intensity of effect

                    // Sample texture twice with slightly offset UV coordinates
                    float4 texColor1 = tex2D(_MainTex, uv + float2(offset, offset));
                    float4 texColor2 = tex2D(_MainTex, uv - float2(offset, offset));

                    // Blend the two sampled colors
                    return (texColor1 + texColor2) / 2.0;
                }
                ENDCG
            }
        }
}
