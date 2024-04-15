#ifndef TOON_PASS_INCLUDED
#define TOON_PASS_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

CBUFFER_START(UnityPerMaterial)
       TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap);
       TEXTURE2D(_ToonShadowMap); SAMPLER(sampler_point_repeat_ToonShadowMap);
       TEXTURE2D(_SmoothnessMap); SAMPLER(sampler_SmoothnessMap);
       float4 _ColorMap_ST;
       float4 _Color;
       float _Smoothness;
       float3 _RimColor;
       float _RimSharpness;
       float _CloseTransparentFactor;
CBUFFER_END

float3 _LightDirection;

struct Attributes
{
       float4 positionOS : POSITION;
       float3 normalOS   : NORMAL;
       float2 uv         : TEXCOORD0;
       
       UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
       float4 positionCS     : SV_POSITION;
       float2 uv              : TEXCOORD0;
       float3 positionWS      : TEXCOORD1;
       float3 normalWS        : TEXCOORD2;
};


float easysmoothstep(float min, float x)
{ 
    return smoothstep(min, min + 0.01, x);
}

Varyings Vertex(Attributes IN)
{
       Varyings OUT = (Varyings)0;

       VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);
       VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);
       
       OUT.positionCS = posInputs.positionCS;
       OUT.positionWS = posInputs.positionWS;
       OUT.uv = TRANSFORM_TEX(IN.uv, _ColorMap);
       OUT.normalWS = normInputs.normalWS;

       return OUT;
}

float FragmentDepthOnly(Varyings IN) : SV_Target
{
       return 0.5;
}

float4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target
{    
       return float4(normalize(IN.normalWS), 0);
}

float3 ToonLighting(Light light,float3 normal,float3 viewDir){

       float NoL = dot(normal, light.direction);
       float2 toonUV = float2(NoL,0);
       

       float optionalEffects = 0;
#ifdef _SPECULAR_ON
       float3 reflectionHalf = normalize(light.direction + viewDir);
       float specular = saturate(dot(normal,reflectionHalf));
       specular = step(0.999*_Smoothness,specular);
       optionalEffects+=specular;
#endif
       float3 toonLight = (SAMPLE_TEXTURE2D(_ToonShadowMap, sampler_point_repeat_ToonShadowMap,toonUV)+optionalEffects)* light.color;

       float2 toonShadowUV = float2(light.shadowAttenuation* light.distanceAttenuation,0);
       float toonShadow = SAMPLE_TEXTURE2D(_ToonShadowMap, sampler_point_repeat_ToonShadowMap,toonShadowUV); 
       // return specular;
       // return light.shadowAttenuation;
       return toonLight * toonShadow;
}


float4 Fragment(Varyings IN) : SV_Target
{ 
           // return float4(normalWS * 0.5 +0.5,1);
       IN.normalWS = normalize(IN.normalWS);
       float2 uv = IN.uv;  
       float4 surfaceColor = _Color * SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
       float3 normalizedViewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
       // float depth = SampleSceneDepth(uv);
       float3 postionWS = IN.positionWS;
       // float3 diff = _WorldSpaceCameraPos-postionWS;
       float tooClose = step(length(_WorldSpaceCameraPos-postionWS)/_CloseTransparentFactor,0.95);

       // return float4(tooClose,0,0,1);
#ifdef _MAIN_LIGHT_SHADOWS_SCREEN
       float4 shadowCoord = ComputeScreenPos(IN.positionCS);
#else
       float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
#endif
       Light mainLight = GetMainLight(shadowCoord); 
       float3 toonLighting = ToonLighting(mainLight,IN.normalWS,normalizedViewDirWS);
       // float3 toonLighting;
       int lightCount = GetAdditionalLightsCount();
       for (int i =0; i< lightCount; i++){
              float4 additionalshadowCoord = TransformWorldToShadowCoord(IN.positionWS);
              Light light = GetAdditionalLight(i,IN.positionWS,0);
              // light.shadowAttenuation = light.direction;
              toonLighting += ToonLighting(light,IN.normalWS,normalizedViewDirWS);
       }

       // return toonLighting;
       return surfaceColor * float4(toonLighting,1-tooClose);
}

half DepthOnlyFragment(Varyings IN) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    return IN.positionCS.z;
}
#endif