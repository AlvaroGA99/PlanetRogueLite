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

        BlitPass blitPass;

        //List<BlitPass> passes = new List<BlitPass>();

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
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
            // CommandBuffer cmd = CommandBufferPool.Get(name);

            // RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            // opaqueDesc.depthBufferBits = 0;

            // var src = origsrc;
            // RenderTargetHandle dest;
            // for (int i = 0; i < passes.Count; i++)
            // {   
            //     if(i == passes.Count -1){
            //         cmd
            //         passes[i].Setup(src,finaldest);
            //         renderer.EnqueuePass(passes[i]);
            //     }else{
            //         dest = new RenderTargetHandle();
            //         dest.Init("_Target"+i);
            //         passes[i].Setup(src,dest);
            //         src = dest.Identifier();
            //         renderer.EnqueuePass(passes[i]);
            //     }
                
            // }
            //dest.Init();

           

            blitPass.Setup(origsrc, finaldest);
            renderer.EnqueuePass(blitPass);
        }

        // public void AddMaterial(Vector3 _dirToSun,Vector3 _planetCentre){
        //     Material aux = Material.Instantiate(settings.blitMaterial);
        //     //blitPass.m_temporaryColorTextureList.Add(blitPass.m_TemporaryColorTexture);
        //     aux.SetVector("_dirToSun",_dirToSun);
        //     aux.SetVector("_planetCentre",_planetCentre);
        //     settings.materialList.Add(aux);
        //     //blitPass.materialList.Add(aux);   
        // }

        // public void ClearMaterials(){
        //     // blitPass.materialList.Clear();
        //     // blitPass.m_temporaryColorTextureList.Clear();
        // }
    }
}