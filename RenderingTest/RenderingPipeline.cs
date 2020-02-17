using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingTest
{
    public static class RenderingPipeline
    {
        /// <summary>
        ///     Gets the <see cref="SpriteBatch"/> instance used for rendering
        ///     sprites and text as batches.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        ///     Gets the <see cref="RenderTarget2D"/> that is used for the final render.
        /// </summary>
        public static RenderTarget2D FinalRenderTarget { get; private set; }

        /// <summary>
        ///     Gets a value indicating if the pipeline has been initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Initialize the pipeline.
        /// </summary>
        /// <param name="gd">
        ///     The <see cref="GraphicsDevice"/> for presenting graphics.
        /// </param>
        public static void Initialize(GraphicsDevice gd)
        {
            SpriteBatch = new SpriteBatch(gd);
            
            FinalRenderTarget = new RenderTarget2D(gd, Engine.Graphics.VirtualWidth, Engine.Graphics.VirtualHeight);

            IsInitialized = true;
        }

        /// <summary>
        ///     Unloads assets used by this.
        /// </summary>
        public static void Unload()
        {
            if(FinalRenderTarget != null && !FinalRenderTarget.IsDisposed)
            {
                FinalRenderTarget.Dispose();
                FinalRenderTarget = null;
            }
        }

        public static void Render(params Renderer[] renderers)
        {
            //  First, we render each of the renderers to their own render targets
            RenderToTargets(renderers);

            //  Next, we render each of the render targets from the renderers to 
            //  the final render target.
            RenderToFinalTarget(renderers);

            //  Last, we render the final rendertarget to screen
            RenderFinalTarget();
        }

        /// <summary>
        ///     Given a collection of <see cref="Renderer"/> instance, render each
        ///     one to their individual <see cref="RenderTarget2D"/> targets.
        /// </summary>
        /// <param name="renderers">
        ///     A collection of <see cref="Renderer"/> instances.
        /// </param>
        private static void RenderToTargets(Renderer[] renderers)
        {
            //  Itereate eachof the renderers given and render them to their own
            //  rendertarget
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];

                Engine.Graphics.SetRenderTarget(renderer.RenderTarget);
                Engine.Graphics.FullViewport();
                Engine.Graphics.Clear(Color.TransparentBlack);

                SpriteBatch.Begin(sortMode: renderer.RendererState.SpriteSortMode,
                                  blendState: renderer.RendererState.BlendState,
                                  samplerState: renderer.RendererState.SamplerState,
                                  depthStencilState: renderer.RendererState.DepthStencilState,
                                  rasterizerState: renderer.RendererState.RasterizerState,
                                  effect: renderer.RendererState.Effect,
                                  transformMatrix: renderer.Camera.TransformationMatrix);

                renderers[i].Render();

                SpriteBatch.End();

                Engine.Graphics.SetRenderTarget(null);
            }


        }

        /// <summary>
        ///     Given a collection of <see cref="Renderer"/> instances, render each
        ///     of their individual <see cref="RenderTarget2D"/> targets to the final
        ///     <see cref="RenderTarget2D"/> target.
        /// </summary>
        /// <param name="renderers">
        ///     A collection of <see cref="Renderer"/> instances.
        /// </param>
        private static void RenderToFinalTarget(Renderer[] renderers)
        {
            Engine.Graphics.SetRenderTarget(FinalRenderTarget);
            Engine.Graphics.Clear(Color.TransparentBlack);

            SpriteBatch.Begin(sortMode: SpriteSortMode.Deferred,
                              blendState: BlendState.AlphaBlend,
                              samplerState: SamplerState.PointClamp,
                              depthStencilState: DepthStencilState.None,
                              rasterizerState: RasterizerState.CullNone,
                              effect: null,
                              transformMatrix: Matrix.Identity);
            for(int i = 0; i < renderers.Length; i++)
            {
                SpriteBatch.Draw(renderers[i].RenderTarget, Vector2.Zero, Color.White);
            }
            SpriteBatch.End();
        }

        /// <summary>
        ///     Renders the final target to the screen.
        /// </summary>
        private static void RenderFinalTarget()
        {
            Engine.Graphics.SetRenderTarget(null);
            Engine.Graphics.Clear();
            Engine.Graphics.ResetViewport();

            //  Use the scale matrix from the graphics 
            SpriteBatch.Begin(sortMode: SpriteSortMode.Deferred,
                              blendState: BlendState.AlphaBlend,
                              samplerState: SamplerState.PointClamp,
                              depthStencilState: DepthStencilState.None,
                              rasterizerState: RasterizerState.CullNone,
                              effect: null,
                              transformMatrix: Engine.Graphics.ScaleMatrix);
            SpriteBatch.Draw(FinalRenderTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        ///     Call when the graphics device is created.  When this happens, all contents of VRAM
        ///     are disacard and render targets need to be recreated.
        /// </summary>
        public static void OnGraphicsCreated()
        {
            FinalRenderTarget = CreateRenderTarget(0, false);
        }

        /// <summary>
        ///     Call when the graphics device is reset.  When this happens, all contents of VRAM
        ///     are disacard and render targets need to be recreated.
        /// </summary>
        public static void OnGraphicsReset()
        {
            FinalRenderTarget = CreateRenderTarget(0, false);
        }

        /// <summary>
        ///     Creates and returns a new <see cref="RenderTarget2D"/> with a width and
        ///     height based on the internal graphics virtual width and virtual heigth.
        /// </summary>
        /// <param name="multiSampleCount">
        ///     A value to give for the preferred MultiSampleCount of the target created.
        /// </param>
        /// <param name="preserved">
        ///     A value indicating if the contents of the target should be preserved or discarded.
        /// </param>
        /// <returns>
        ///     A new <see cref="RenderTarget2D"/> instance.
        /// </returns>
        public static RenderTarget2D CreateRenderTarget(int multiSampleCount, bool preserved)
        {
            return new RenderTarget2D(graphicsDevice: Engine.Graphics.Device,
                                     width: Engine.Graphics.VirtualWidth,
                                     height: Engine.Graphics.VirtualHeight,
                                     mipMap: false,
                                     preferredFormat: SurfaceFormat.Color,
                                     preferredDepthFormat: DepthFormat.None,
                                     preferredMultiSampleCount: multiSampleCount,
                                     usage: preserved ? RenderTargetUsage.PreserveContents : RenderTargetUsage.DiscardContents);
        }

    }
}
