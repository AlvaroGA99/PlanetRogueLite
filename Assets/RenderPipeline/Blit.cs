using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Experimental.Rendering.Universal
{
    public class Blit : ScriptableRendererFeature
    {
        [System.Serializable]
        public class BlitSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

            public Material blitMaterial = null;
            public Material blitMaterial1 = null;
            public int blitMaterialPassIndex = -1;
            public Target destination = Target.Color;
            public string textureId = "_BlitPassTexture";
            public List<Material> materialList = new List<Material>();
        }

        public enum Target
        {
            Color,
            Texture
        }

        public BlitSettings settings = new BlitSettings();
        RenderTargetHandle m_RenderTextureHandle;

        List<BlitPass> passes;
        BlitPass blitPass;
        BlitPass blitPass1;

        //List<BlitPass> passes = new List<BlitPass>();

        public override void Create()
        {   

            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            if (settings.materialList.Count > 0){
                passes = new List<BlitPass>();
                foreach(Material m in settings.materialList){
                    passes.Add(new BlitPass(settings.Event, m, settings.blitMaterialPassIndex, name));
                }
            }else{
                passes = new List<BlitPass>();
                blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
                blitPass1 = new BlitPass(settings.Event, settings.blitMaterial1, settings.blitMaterialPassIndex, name);
                passes.Add(blitPass);
                passes.Add(blitPass1);
            }
            
            
            //passes.Add(blitPass);
            
           
            m_RenderTextureHandle.Init(settings.textureId);
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var origsrc = renderer.cameraColorTarget;
            var finaldest = (settings.destination == Target.Color) ? RenderTargetHandle.CameraTarget : m_RenderTextureHandle;

             if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }
            // blitPass0.Setup(origsrc, finaldest);
            // blitPass0.blitMaterial.SetInteger("_index",0);
            // renderer.EnqueuePass(blitPass0);
            foreach(BlitPass bp in passes){
                bp.Setup(origsrc,finaldest);
                renderer.EnqueuePass(bp);
            }
            // blitPass.Setup(origsrc, finaldest);
            // blitPass.blitMaterial.SetInteger("_index",1);
            // renderer.EnqueuePass(blitPass);
            // blitPass1.Setup(origsrc, finaldest);
            // renderer.EnqueuePass(blitPass1);
        }
    }
}