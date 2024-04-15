using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderCustomInspector : ShaderGUI {

    public enum SurfaceType {
        Opaque, TransparentBlend, TransparentCutout
    }
    
    public enum FaceRenderingMode {
        FrontOnly, NoCulling, DoubleSided
    }

    public enum BlendType {
        Alpha,
        Premultiplied,
        Additive,
        Multiply
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader) {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if(newShader.name == "MyShaders/Test") {
            UpdateSurfaceType(material);
        }
    }
    
#if UNITY_2022_1_OR_NEWER
    public override void ValidateMaterial(Material material) {
        base.ValidateMaterial(material);
        UpdateSurfaceType(material);
    }
#endif

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {

        Material material = materialEditor.target as Material;
        var surfaceProp = BaseShaderGUI.FindProperty("_SurfaceType", properties, true);
        var faceProp = BaseShaderGUI.FindProperty("_FaceRenderingMode", properties, true);
        var blendProp = BaseShaderGUI.FindProperty("_BlendType",properties,true);
        
        EditorGUI.BeginChangeCheck();
        
        MaterialEditor.BeginProperty(surfaceProp);
        surfaceProp.floatValue = (int)(SurfaceType)EditorGUILayout.EnumPopup("Surface type", (SurfaceType)surfaceProp.floatValue);
        MaterialEditor.EndProperty();
        
        MaterialEditor.BeginProperty(blendProp);
        blendProp.floatValue = (int)(BlendType)EditorGUILayout.EnumPopup("Blend type", (BlendType)blendProp.floatValue);
        MaterialEditor.EndProperty();

        MaterialEditor.BeginProperty(faceProp);
        faceProp.floatValue = (int)(FaceRenderingMode)EditorGUILayout.EnumPopup("Face rendering mode", (FaceRenderingMode)faceProp.floatValue);
        MaterialEditor.EndProperty();
    
        if(EditorGUI.EndChangeCheck()) {
            UpdateSurfaceType(material);
        }

        base.OnGUI(materialEditor, properties);
    }

    private void UpdateSurfaceType(Material material) {
        SurfaceType surface = (SurfaceType)material.GetFloat("_SurfaceType");

        BlendType blend = (BlendType)material.GetFloat("_BlendType");
        switch(surface) {
        case SurfaceType.Opaque:
            material.renderQueue = (int)RenderQueue.Geometry;
            material.SetOverrideTag("RenderType", "Opaque");
            break;
        case SurfaceType.TransparentCutout:
            material.renderQueue = (int)RenderQueue.AlphaTest;
            material.SetOverrideTag("RenderType", "TransparentCutout");
            break;
        case SurfaceType.TransparentBlend:
            material.renderQueue = (int)RenderQueue.Transparent;
            material.SetOverrideTag("RenderType", "Transparent");
            break;
        }

        switch(surface) {
        case SurfaceType.Opaque:
        case SurfaceType.TransparentCutout:
            material.SetInt("_SourceBlend", (int)BlendMode.One);
            material.SetInt("_DestBlend", (int)BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            break;
        case SurfaceType.TransparentBlend:
            switch(blend){
                case BlendType.Alpha:
                    material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                    material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendType.Premultiplied:
                    material.SetInt("_SourceBlend", (int)BlendMode.One);
                    material.SetInt("_DestBlend", (int)BlendMode.OneMinusSrcAlpha);
                    break;
                case BlendType.Multiply:
                    material.SetInt("_SourceBlend", (int)BlendMode.Zero);
                    material.SetInt("_DestBlend", (int)BlendMode.SrcColor);
                    break;
                case BlendType.Additive:
                    material.SetInt("_SourceBlend", (int)BlendMode.SrcAlpha);
                    material.SetInt("_DestBlend", (int)BlendMode.One);
                    break;
            }
            material.SetInt("_ZWrite", 0);
            break;
        }

        if(surface == SurfaceType.TransparentBlend && blend == BlendType.Premultiplied) {
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        } else {
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if(surface == SurfaceType.TransparentCutout) {
            material.EnableKeyword("_ALPHA_CUTOUT");
        } else {
            material.DisableKeyword("_ALPHA_CUTOUT");
        }

        material.SetShaderPassEnabled("ShadowCaster", surface != SurfaceType.TransparentBlend);
        
        FaceRenderingMode faceRenderingMode = (FaceRenderingMode)material.GetFloat("_FaceRenderingMode");
        if(faceRenderingMode == FaceRenderingMode.FrontOnly) {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
        } else {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }

        if(faceRenderingMode == FaceRenderingMode.DoubleSided) {
            material.EnableKeyword("_DOUBLE_SIDED_NORMALS");
        } else {
            material.DisableKeyword("_DOUBLE_SIDED_NORMALS");
        }
    }
}