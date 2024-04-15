Shader "Custom/Toon"
{
    Properties
    {
        [Toggle(_SPECULAR_ON)] _SpecularToggle("Specular Highlights",Float) = 0
        [Toggle(_RIMLIGHTS_ON)] _RimLightToggle("Rim Lighting",Float) = 0

        [MainColor] _Color("Color", Color) = (1,1,1,1)
        [HDR] _RimColor ("Rim Color", Color) = (1.0, 1.0, 1.0)
        _Smoothness("Smoothness", Range(0.5,1.5)) = 0.5
        _RimSharpness("Rim Sharpness", Range(1,15)) = 0.5
        [MainTexture] _ColorMap("Color Map",2D) = "white" {}
        _ToonShadowMap("Toon Shadow Map",2D) = "white" {}
        [NoScaleOffset]_SmoothnessMap("Smoothness Mask",2D) = "white" {}
        _CloseTransparentFactor("_CloseTransparentFactor", Float) = 3

        [HideInInspector] _Cull("Cull mode", Float) = 2 // 2 is "Back"
        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0
        [HideInInspector] _SurfaceType("Surface type", Float) = 0
        [HideInInspector] _FaceRenderingMode("Face rendering type", Float) = 0
        [HideInInspector] _BlendType("Blend Type", Float) = 0

    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            Blend[_SourceBlend][_DestBlend]
            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM

            // #define _SPECULAR_COLOR
            #define _NORMALMAP
            #define _CLEARCOATMAP
            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS

            #pragma shader_feature_local_fragment _SPECULAR_ON
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            
            #pragma multi_compile_fragment _ _DEBUG_DISPLAY

            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "ToonPass.hlsl"
            
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            ColorMask 0

            
            HLSLPROGRAM

            #define SHADOW_CASTER_PASS
            
            #pragma vertex Vertex
            #pragma fragment FragmentDepthOnly
            
            #include "ToonPass.hlsl"
            
            ENDHLSL
            
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex Vertex
            #pragma fragment DepthOnlyFragment

            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #include "ToonPass.hlsl"

            ENDHLSL
        }

    }
    CustomEditor "ShaderCustomInspector"
}
