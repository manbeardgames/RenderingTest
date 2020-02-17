using Microsoft.Xna.Framework.Graphics;

namespace RenderingTest
{
    /// <summary>
    ///     A simple class used to represent the different state values used by a <see cref="Renderer"/>
    ///     when beginning a <see cref="SpriteBatch"/>.
    /// </summary>
    public class RendererState
    {
        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.SamplerState"/>
        ///     to use when rendering.
        /// </summary>
        public SpriteSortMode SpriteSortMode { get; set; }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.BlendState"/> 
        ///     to use when rendering.
        /// </summary>
        public BlendState BlendState { get; set; }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.SamplerState"/> 
        ///     to use when rendering.
        /// </summary>
        public SamplerState SamplerState { get; set; }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.DepthStencilState"/> 
        ///     to use when rendering.
        /// </summary>
        public DepthStencilState DepthStencilState { get; set; }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.RasterizerState"/> 
        ///     to use when rendering.
        /// </summary>
        public RasterizerState RasterizerState { get; set; }

        /// <summary>
        ///     Get or Sets the <see cref="Microsoft.Xna.Framework.Graphics.Effect"/>
        ///     to use when rendering.
        /// </summary>
        public Effect Effect { get; set; }
    }
}
