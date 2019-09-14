Shader "VFX/Volumetric" {
Properties {
    _MainTex ("Particle Texture", 2D) = "white" {}
    [HDR]_C("Tint Color", Color) = (1,1,1,1)
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.5
    _RP ("Rim Fade Power", Range(0.1, 8)) = 8
    _AM ("Alpha Multiplier", Range(0,3)) = 1
    _P("Fall Off Power", Range(0,10)) = 1
    _FO ("Fall Off", Range(0, 10)) = 10
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend One OneMinusSrcColor
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;
            float _RP;
            float _AM;
            float _P;
            float _FO;
            float4 _C;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                // apply inverse rim color, to fade out normals away from camera
                half rim = dot (normalize(ObjSpaceViewDir(v.vertex)), v.normal);
                o.color = fixed4(v.color.r, v.color.g, v.color.b, v.color.a *_AM * smoothstep(1 - _RP, 1, rim));

                // apply alpha fall off based on local z distance
                o.color.a = clamp(o.color.a * lerp(_P,0, (v.vertex.z * -1) / _FO), 0, 1);  

                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            sampler2D_float _CameraDepthTexture;
            float _InvFade;
            
            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif
                
                half4 col = i.color * _C * tex2D(_MainTex, i.texcoord);
                col.rgb *= col.a;
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
                return col;
            }
            ENDCG 
        }
    } 
}
}