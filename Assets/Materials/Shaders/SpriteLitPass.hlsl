#ifndef SPRITE_LIT_PASS_INCLUDED
#define SPRITE_LIT_PASS_INCLUDED


#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float3 positionOS   : POSITION;
    float4 color        : COLOR;
    float2 uv           : TEXCOORD0;
    float3 normalOS     : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4  positionCS      : SV_POSITION;
    float4  color           : COLOR;
    float2  uv              : TEXCOORD0;
    float3  positionWS  : TEXCOORD1;
    float3  normalWS        : TEXCOORD2;
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_ToonShadowMap); SAMPLER(sampler_point_repeat_ToonShadowMap);
TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

float4 _MainTex_ST;
float4 _Color;
half4 _RendererColor;
float3 _EmissionTint;



float4 SpriteLighting(Light light,float3 normal,float3 viewDir){

        float NoL = max(dot(normal, light.direction),
        dot(float3(normal.x,-normal.y,normal.z), light.direction));
        float2 toonUV = float2(NoL,0);
        

        float optionalEffects = 0;
        float3 toonLight = (SAMPLE_TEXTURE2D(_ToonShadowMap, sampler_point_repeat_ToonShadowMap,toonUV)+optionalEffects)* light.color;

        float toonShadowUV = float2(light.shadowAttenuation* light.distanceAttenuation,0);
        float toonShadow = SAMPLE_TEXTURE2D(_ToonShadowMap, sampler_point_repeat_ToonShadowMap,toonShadowUV); 
        return float4(toonLight * toonShadow,1);
}

Varyings Vertex(Attributes IN)
{
    
    Varyings OUT = (Varyings)0;

    VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);
    UNITY_SETUP_INSTANCE_ID(IN);
    
    OUT.positionCS = posInputs.positionCS;
    OUT.positionWS = posInputs.positionWS;
    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
    OUT.normalWS = normInputs.normalWS;
    OUT.color = IN.color;

    return OUT;
}

float4 Fragment(Varyings IN) : SV_Target
{
    float4 mainTex = IN.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
    // return mainTex;
    // return float4(1,1,1,1);
    IN.normalWS = normalize(IN.normalWS);
    float2 uv = IN.uv;  
    float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);


#ifdef _MAIN_LIGHT_SHADOWS_SCREEN
    float4 shadowCoord = ComputeScreenPos(IN.positionCS);
    
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
    #endif
    Light mainLight = GetMainLight(shadowCoord); 
    // return float4(IN.normalWS,1);
    float4 spriteLighting = SpriteLighting(mainLight,float3(0,1,0),viewDirWS);
    // float4 spriteLighting;
    int lightCount = GetAdditionalLightsCount();
    for (int i =0; i< lightCount; i++){
           float4 additionalshadowCoord = TransformWorldToShadowCoord(IN.positionWS);
           Light light = GetAdditionalLight(i,IN.positionWS,0);
           // light.shadowAttenuation = light.direction;
           spriteLighting += SpriteLighting(light,float3(0,1,0),viewDirWS);
        //    spriteLighting += SpriteLighting(light,-IN.normalWS,viewDirWS);
    }

    float3 emissive =  SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap, uv).rgb * _EmissionTint;


    // return mainTex;
    return float4(spriteLighting.rgb*mainTex.rgb*emissive,step(0.01,mainTex.a)*mainTex.a);

}


float FragmentDepthOnly(Varyings IN) : SV_Target
{
       return 0;
}

#endif