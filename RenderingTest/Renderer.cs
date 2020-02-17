using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenderingTest
{
    public class Renderer
    {
        //  The texture to render. This is just here for this test.  In production,
        //  this renderer will have a method of detecting which objects it should
        //  be rendering. 
        private Texture2D _texture;

        /// <summary>
        ///     Gets the <see cref="RenderTarget2D"/> this renderer renders to.
        /// </summary>
        public RenderTarget2D RenderTarget { get; protected set; }

        /// <summary>
        ///     Gets or Sets the state value this renderer uses when beginning a 
        ///     <see cref="SpriteBatch"/>.
        /// </summary>
        public RendererState RendererState { get; set; }

        /// <summary>
        ///     Gets or Sets the camera used for translation, rotation, and scale
        ///     of the render.
        /// </summary>
        public Camera2D Camera { get; set; }

        /// <summary>
        ///     Creates a new <see cref="Renderer"/> instance.
        /// </summary>
        /// <param name="width">
        ///     The width of the render in pixels.
        /// </param>
        /// <param name="height">
        ///     The height of the render in pixels.
        /// </param>
        /// <param name="texture">
        ///     The <see cref="Texture2D"/> to render
        /// </param>
        public Renderer(int width, int height, Texture2D texture)
        {

            Camera = new Camera2D(width, height);
            RendererState = new RendererState()
            {
                SpriteSortMode = SpriteSortMode.Deferred,
                BlendState = BlendState.AlphaBlend,
                SamplerState = SamplerState.PointClamp,
                DepthStencilState = DepthStencilState.None,
                RasterizerState = RasterizerState.CullNone,
                Effect = null
            };
            _texture = texture;
            RenderTarget = RenderingPipeline.CreateRenderTarget(0, false);
        }

        /// <summary>
        ///     Called when the graphics device is created.  When this occurs, contents of
        ///     VRAM are wiped and thigns such as render targets need to be recreated.
        /// </summary>
        public void OnGraphicsCreated()
        {
            RenderTarget = RenderingPipeline.CreateRenderTarget(0, false);
        }

        /// <summary>
        ///     Called when the graphics device is reset.  When this occurs, contents of
        ///     VRAM are wiped and thigns such as render targets need to be recreated.
        /// </summary>
        public void OnGraphicsReset()
        {
            RenderTarget = RenderingPipeline.CreateRenderTarget(0, false);
        }

        /// <summary>
        ///     Renders this renderer.
        /// </summary>
        public virtual void Render()
        {
            RenderingPipeline.SpriteBatch.Draw(_texture, Vector2.Zero, Color.White);
        }

        /// <summary>
        ///     Unloads the resources used by this renderer.
        /// </summary>
        public void Unload()
        {
            if (RenderTarget != null && !RenderTarget.IsDisposed)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }
        }


        // --------------------------------------
        //  ... other code removed for brevity.
        // --------------------------------------
    }
}
