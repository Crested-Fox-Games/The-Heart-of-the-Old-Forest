// Procedural day/night skybox: gradient sky, sunset glow, sun, moon, twinkling stars, scrolling clouds.
// Works in Built-in and URP (not HDRP). No textures needed.
// Driven by DynamicSkyController.cs, which sets _SunDir, _MoonDir and _StarRotation globally.
Shader "Skybox/DynamicSky"
{
    Properties
    {
        [Header(Sky Colours)]
        _DayTop ("Day Top", Color) = (0.22, 0.45, 0.9, 1)
        _DayHorizon ("Day Horizon", Color) = (0.65, 0.8, 0.95, 1)
        _NightTop ("Night Top", Color) = (0.005, 0.01, 0.03, 1)
        _NightHorizon ("Night Horizon", Color) = (0.03, 0.05, 0.1, 1)
        _SunsetColor ("Sunset Colour", Color) = (1, 0.4, 0.15, 1)
        _HorizonFalloff ("Horizon Falloff", Range(0.5, 8)) = 3
        _GroundColor ("Below Horizon", Color) = (0.15, 0.15, 0.17, 1)

        [Header(Sun)]
        _SunColor ("Sun Colour", Color) = (1, 0.95, 0.85, 1)
        _SunSize ("Sun Size", Range(0.0001, 0.01)) = 0.0015
        _SunGlow ("Sun Glow", Range(0, 2)) = 0.6

        [Header(Moon)]
        _MoonColor ("Moon Colour", Color) = (0.8, 0.85, 1, 1)
        _MoonSize ("Moon Size", Range(0.0001, 0.01)) = 0.001
        _MoonGlow ("Moon Glow", Range(0, 1)) = 0.15

        [Header(Stars)]
        _StarDensity ("Star Density", Range(50, 500)) = 250
        _StarAmount ("Star Amount", Range(0, 0.1)) = 0.02
        _StarBrightness ("Star Brightness", Range(0, 5)) = 2

        [Header(Clouds)]
        _CloudColor ("Cloud Lit Colour", Color) = (1, 1, 1, 1)
        _CloudShadow ("Cloud Shadow Colour", Color) = (0.55, 0.6, 0.7, 1)
        _CloudScale ("Cloud Scale", Range(0.1, 5)) = 1.5
        _CloudCoverage ("Cloud Coverage", Range(0, 1)) = 0.45
        _CloudSoftness ("Cloud Softness", Range(0.01, 0.5)) = 0.15
        _CloudSpeed ("Cloud Wind (XY)", Vector) = (0.02, 0.01, 0, 0)

        [Header(Mood)]
        _Saturation ("Saturation", Range(0, 1)) = 1
        _HorizonHaze ("Horizon Haze", Range(0, 1)) = 0.5
        _HazeHeight ("Haze Height", Range(0.01, 1)) = 0.25
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _DayTop, _DayHorizon, _NightTop, _NightHorizon, _SunsetColor, _GroundColor;
            float _HorizonFalloff;
            float4 _SunColor; float _SunSize, _SunGlow;
            float4 _MoonColor; float _MoonSize, _MoonGlow;
            float _StarDensity, _StarAmount, _StarBrightness;
            float4 _CloudColor, _CloudShadow, _CloudSpeed;
            float _CloudScale, _CloudCoverage, _CloudSoftness;
            float _Saturation, _HorizonHaze, _HazeHeight;

            // Set globally by DynamicSkyController
            float3 _SkyFogColor;
            float _TwilightWidth;
            float3 _SunDir;
            float3 _MoonDir;
            float4x4 _StarRotation;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            float Hash13(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.zyx + 31.32);
                return frac((p.x + p.y) * p.z);
            }

            float Hash12(float2 p)
            {
                float3 p3 = frac(p.xyx * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = Hash12(i);
                float b = Hash12(i + float2(1, 0));
                float c = Hash12(i + float2(0, 1));
                float d = Hash12(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float Fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 5; i++)
                {
                    v += a * Noise(p);
                    p *= 2.02;
                    a *= 0.5;
                }
                return v;
            }

            float Stars(float3 dir)
            {
                float3 p = dir * _StarDensity;
                float3 cell = floor(p);
                float3 local = frac(p) - 0.5;
                float h = Hash13(cell);
                if (h > _StarAmount) return 0.0;
                float bright = Hash13(cell + 17.0);
                float twinkle = 0.75 + 0.25 * sin(_Time.y * (2.0 + bright * 4.0) + h * 600.0);
                return smoothstep(0.35, 0.0, length(local)) * bright * twinkle;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);
                float3 sunDir = normalize(_SunDir);
                float3 moonDir = normalize(_MoonDir);

                float tw = max(_TwilightWidth, 0.01);
                float dayFactor = smoothstep(-tw, tw, sunDir.y);
                float sunsetFactor = saturate(1.0 - abs(sunDir.y) / (tw * 1.7));
                float aboveHorizon = smoothstep(-0.01, 0.01, dir.y);

                // --- Sky gradient ---
                float horizonT = pow(1.0 - saturate(dir.y), _HorizonFalloff);
                float3 dayCol = lerp(_DayTop.rgb, _DayHorizon.rgb, horizonT);
                float3 nightCol = lerp(_NightTop.rgb, _NightHorizon.rgb, horizonT);
                float3 sky = lerp(nightCol, dayCol, dayFactor);

                // Sunset/sunrise glow, strongest on the sun's side of the horizon
                float sunDot = dot(dir, sunDir);
                float sunSide = pow(saturate(sunDot * 0.5 + 0.5), 3.0);
                sky = lerp(sky, _SunsetColor.rgb, sunsetFactor * horizonT * sunSide);

                // Below the horizon
                float3 ground = _GroundColor.rgb * lerp(0.05, 1.0, dayFactor);
                sky = lerp(sky, ground, smoothstep(0.0, 0.05, -dir.y));

                float3 col = sky;

                // --- Stars (rotate with the night sky) ---
                float3 starDir = mul((float3x3)_StarRotation, dir);
                float nightVis = (1.0 - dayFactor) * smoothstep(-0.02, 0.1, dir.y);
                col += Stars(starDir) * _StarBrightness * nightVis;

                // --- Moon ---
                float moonDot = dot(dir, moonDir);
                float moonDisc = smoothstep(1.0 - _MoonSize, 1.0 - _MoonSize * 0.7, moonDot) * aboveHorizon;
                float moonGlow = pow(saturate(moonDot), 300.0) * _MoonGlow * aboveHorizon;
                col = lerp(col, max(col, _MoonColor.rgb * 1.2), moonDisc);
                col += _MoonColor.rgb * moonGlow * (1.0 - dayFactor * 0.8);

                // --- Sun disc ---
                float sunDisc = smoothstep(1.0 - _SunSize, 1.0 - _SunSize * 0.7, sunDot) * aboveHorizon;
                col += _SunColor.rgb * sunDisc * 4.0;

                // --- Clouds (flat layer projected onto the dome) ---
                float2 cloudUV = dir.xz / (dir.y + 0.15) * _CloudScale + _Time.y * _CloudSpeed.xy;
                float n = Fbm(cloudUV);
                float threshold = 1.0 - _CloudCoverage;
                float cloud = smoothstep(threshold - _CloudSoftness, threshold + _CloudSoftness, n);
                cloud *= smoothstep(0.0, 0.15, dir.y);

                float3 cloudCol = lerp(_CloudShadow.rgb, _CloudColor.rgb, saturate((n - 0.3) * 2.0));
                float3 dayCloud = lerp(cloudCol, cloudCol * _SunsetColor.rgb * 1.3, sunsetFactor * 0.7);
                dayCloud += _SunColor.rgb * pow(saturate(sunDot), 16.0) * 0.5;
                float3 nightCloud = cloudCol * _NightHorizon.rgb * 3.0 + _MoonColor.rgb * pow(saturate(moonDot), 8.0) * 0.1;
                cloudCol = lerp(nightCloud, dayCloud, dayFactor);

                col = lerp(col, cloudCol, cloud);

                // --- Horizon haze: blends the sky into the scene fog colour ---
                float hazeT = saturate(1.0 - dir.y / _HazeHeight);
                float below = saturate(-dir.y * 20.0) * saturate(_HorizonHaze * 2.0);
                float haze = lerp(hazeT * hazeT * _HorizonHaze, 1.0, below);
                col = lerp(col, _SkyFogColor, haze);

                // --- Sun glow sits on top of clouds, softened by haze ---
                float sunGlow = (pow(saturate(sunDot), 200.0) + pow(saturate(sunDot), 8.0) * 0.15) * _SunGlow;
                col += _SunColor.rgb * sunGlow * smoothstep(-0.1, 0.05, sunDir.y) * (1.0 - haze * 0.5);

                // --- Desaturate for mood ---
                float lum = dot(col, float3(0.2126, 0.7152, 0.0722));
                col = lerp(lum.xxx, col, _Saturation);

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
