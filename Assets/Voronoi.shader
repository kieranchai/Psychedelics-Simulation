Shader "Custom/ScrollingSwirlingTextureShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _IsEnabled("Is Enabled", Float) = 0.0
        _ScrollSpeed("Scroll Speed", Float) = 1.0
        _SwirlStrength("Swirl Strength", Float) = 1.0
        _Color("Color", Color) = (1, 1, 1, 1)  // Default color is white
        _EnableColorPulsing("Enable Color Pulsing", Float) = 0.0
        _PulseSpeed("Pulse Speed", Float) = 1.0
        _DarkSaturation("Dark Saturation", Color) = (0.5, 0.5, 0.5, 1.0)
        _LightSaturation("Light Saturation", Color) = (1.5, 1.5, 1.5, 1.0)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            struct Input
            {
                float2 uv_MainTex;
            };

            float _ScrollSpeed;
            float _SwirlStrength;
            float _EnableColorPulsing;
            float _PulseSpeed;
            fixed4 _Color;
            fixed4 _DarkSaturation;
            fixed4 _LightSaturation;
            sampler2D _MainTex;

            void surf(Input IN, inout SurfaceOutput o)
            {
                // Calculate scrolling offset
                float offsetX = _Time.y * _ScrollSpeed;
                float2 offsetUV = IN.uv_MainTex + float2(offsetX, 0);

                // Calculate swirl effect
                float swirlAmount = length(offsetUV - 0.5); // Distance from center
                float angle = swirlAmount * _SwirlStrength * _Time.y; // Swirling angle
                float2 swirlOffset = float2(cos(angle), sin(angle)) * swirlAmount * _SwirlStrength;
                offsetUV += swirlOffset;

                // Apply color pulsing effect
                if (_EnableColorPulsing > 0.5)
                {
                    float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                    fixed4 targetColor = lerp(_DarkSaturation, _LightSaturation, pulse);
                    _Color = targetColor;
                }

                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, offsetUV);

                // Apply color property
                texColor *= _Color;

                o.Albedo = texColor.rgb;
                o.Alpha = texColor.a;
            }
            ENDCG
        }

            FallBack "Diffuse"
}
