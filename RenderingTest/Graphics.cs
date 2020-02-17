using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RenderingTest
{
    public class Graphics
    {
        //  A value indicating if the scale matrix is dirty and needs to be
        //  recalculated
        private bool _dirtyMatrix;

        //  The scale matrix value to use with the spritebatch when rendering a 
        //  final render to the screen.  Describes how to scale the render to accommodate the
        //  virtual width and height with the backbuffer width and height.
        private Matrix _scaleMatrix;

        //  The graphics device manager used to control the presentation of graphics.
        private GraphicsDeviceManager _manager;

        //  THe game window used to display the game.
        private GameWindow _window;

        //  An action to execute when the graphics device is created.
        private Action _onDeviceCreated;

        //  An action to execute when the graphics device is reset.
        private Action _onDeviceReset;

        //  An action to execute after the client (window) size is changed.
        private Action _onClientSizeChanged;

        //  A value indicating if the client (window) is current resizing.
        private bool _isResizing;

        //  The base game class of our game.
        private Game _game;

        /// <summary>
        ///     Gets the GraphicsDevice used to present graphics.
        /// </summary>
        public GraphicsDevice Device
        {
            get { return _manager.GraphicsDevice; }
        }

        /// <summary>
        ///     Gets the width in pixels of the backbuffer.  This is also 1:1 with the
        ///     window width.
        /// </summary>
        public int BackBufferWidth { get; private set; }

        /// <summary>
        ///     Gets the height in pixels of the backbuffer.  This is also 1:1 with the
        ///     window height.
        /// </summary>
        public int BackBufferHeight { get; private set; }

        /// <summary>
        ///     Gets the virtual width in pixels.  This is the width that everything should
        ///     be rendered to and concerned with in game logic before rendering to the screen.
        /// </summary>
        public int VirtualWidth { get; private set; }

        /// <summary>
        ///     Gets the virtual height in pixels. This is the heigh that everything should
        ///     be rendered to and concerned with in game logic before rendering to the screen.
        /// </summary>
        public int VirtualHeight { get; private set; }

        /// <summary>
        ///     Gets a value representing the aspect ratio of the virtual resolution.
        /// </summary>
        public float VirtualAspectRatio
        {
            get { return (float)VirtualWidth / (float)VirtualHeight; }
        }

        /// <summary>
        ///     Gets a value indicating if the graphics are currently rendering in fullscreen mode.
        /// </summary>
        public bool Fullscreen { get; private set; }

        /// <summary>
        ///     Gets the scale matrix to apply to the spritebatch when rendering to scale the 
        ///     virtual render to the actual render resolution.  
        /// </summary>
        public Matrix ScaleMatrix
        {
            get
            {
                if (_dirtyMatrix)
                {
                    RecreateScaleMatrix();
                }

                return _scaleMatrix;
            }
        }

        /// <summary>
        ///     Gets or Sets the clear color to clear the buffer to when one isn't specified.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        ///     Creates a new <see cref="Graphics"/> instance.
        /// </summary>
        /// <param name="game">
        ///     The base <see cref="Game"/> class of our game.
        /// </param>
        public Graphics(Game game)
        {
            _game = game;
            _window = game.Window;
            ClearColor = Color.Black;
        }

        public void Initialize(int backbufferWidth,
                               int backbufferHeight,
                               int virtualWidth,
                               int virtualHeight,
                               bool fullscreen,
                               Action onGraphicsCreated = null,
                               Action onGraphicsReset = null,
                               Action onClientSizeChanged = null)
        {
            BackBufferWidth = backbufferWidth > 0 ? backbufferWidth
                : throw new ArgumentOutOfRangeException("The backbuffer width must be greater than zero", nameof(backbufferWidth));

            BackBufferHeight = backbufferHeight > 0 ? backbufferHeight
                : throw new ArgumentOutOfRangeException("The backbuffer width must be greater than zero", nameof(backbufferHeight));

            VirtualWidth = virtualWidth > 0 ? virtualWidth
                : throw new ArgumentOutOfRangeException("The backbuffer width must be greater than zero", nameof(virtualWidth));

            VirtualHeight = virtualHeight > 0 ? virtualHeight
                : throw new ArgumentOutOfRangeException("The backbuffer width must be greater than zero", nameof(virtualHeight));

            //  Create the graphics device manager
            _manager = new GraphicsDeviceManager(_game);

            //  Cache the actions to execute for the events we'll bind
            _onDeviceCreated = onGraphicsCreated;
            _onDeviceReset = onGraphicsReset;
            _onClientSizeChanged = onClientSizeChanged;

            //  Bind to the events
            _manager.DeviceCreated += DeviceCreated;
            _manager.DeviceReset += DeviceReset;
            _window.ClientSizeChanged += ClientSizeChanged;

            //  Set the default properties of the device manager
            _manager.SynchronizeWithVerticalRetrace = true;
            _manager.PreferMultiSampling = false;
            _manager.GraphicsProfile = GraphicsProfile.HiDef;
            _manager.PreferredBackBufferFormat = SurfaceFormat.Color;
            _manager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            _window.AllowUserResizing = true;

            //  Set the matrix as dirty initially to force the matrix to recalculate
            _dirtyMatrix = true;

            //  Set the initial resolution
            SetResolution(backbufferWidth, backbufferHeight, fullscreen);

            //  Set the initial virtual resolution
            SetVirtualResolution(virtualWidth, virtualHeight);
        }

        /// <summary>
        ///     Called when the graphics device is created.
        /// </summary>
        private void DeviceCreated(object sender, EventArgs e)
        {
            _onDeviceCreated?.Invoke();
        }

        /// <summary>
        ///     Called when the graphics device is reset.
        /// </summary>
        private void DeviceReset(object sender, EventArgs e)
        {
            _onDeviceReset?.Invoke();
        }

        /// <summary>
        ///     Called after the client (window) size is changed.
        /// </summary>
        private void ClientSizeChanged(object sender, EventArgs e)
        {
            _onClientSizeChanged?.Invoke();
        }

        /// <summary>
        ///     Given a width and height, sets the internal resolution of the
        ///     graphics.
        /// </summary>
        /// <param name="width">
        ///     The width of the resolution in pixels.
        ///     Must be greater than 0
        /// </param>
        /// <param name="height">
        ///     The height of the resolution in pixels.
        ///     Must be greater than 0
        /// </param>
        /// <param name="fullscreen">
        ///     A value indicating if we should be in full screen mode.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if either <paramref name="width"/> or <paramref name="height"/> are
        ///     less than or equal to zero.
        /// </exception>
        public void SetResolution(int width, int height, bool fullscreen)
        {
            BackBufferWidth = width > 0 ? width
                : throw new ArgumentOutOfRangeException("The resolution width must be greater than 0", nameof(width));

            BackBufferHeight = height > 0 ? height
                : throw new ArgumentOutOfRangeException("The resolution height must be greater than 0", nameof(height));

            Fullscreen = fullscreen;
            ApplyResolutionSettings();
        }

        /// <summary>
        ///     Given a width and height, sets the internal virtual resolution.
        /// </summary>
        /// <param name="width">
        ///     The width of the virtual resolution in pixels.
        ///     Must be greater than zero.
        /// </param>
        /// <param name="height">
        ///     The height of the virtual resolution in pixels.
        ///     Must be greater than zero.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if either <paramref name="width"/> or <paramref name="height"/> are
        ///     less than or equal to zero.
        /// </exception>
        public void SetVirtualResolution(int width, int height)
        {
            VirtualWidth = width > 0 ? width
                : throw new ArgumentOutOfRangeException("The virtual width must be greater than 0", nameof(width));

            VirtualHeight = height > 0 ? height
                : throw new ArgumentOutOfRangeException("The virtual height must be greater than 0", nameof(height));

            _dirtyMatrix = true;
        }

        private void ApplyResolutionSettings()
        {
            //  If we aren't using fullscreen mode, the width and heigh tof hte window can be set
            //  to anything equal to or smaller than the actual screen size.
            if (!Fullscreen)
            {
                if (BackBufferWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width &&
                   BackBufferHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                {
                    _manager.PreferredBackBufferWidth = BackBufferWidth;
                    _manager.PreferredBackBufferHeight = BackBufferHeight;
                    _manager.IsFullScreen = Fullscreen;
                    _manager.ApplyChanges();
                }
            }
            else
            {
                //  If we are using fullscreen mode, we should check to make sure that the display
                //  adapter can handle the video mode we are trying to set.  To do this, we will
                //  iterate through the display modes supported by the adapter and check them
                //  against the mode we want to set.
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (mode.Width == BackBufferWidth && mode.Height == BackBufferHeight)
                    {
                        //  The mode is supported, so set the buffer formats, apply changes, and return
                        _manager.PreferredBackBufferWidth = BackBufferWidth;
                        _manager.PreferredBackBufferHeight = BackBufferHeight;
                        _manager.IsFullScreen = Fullscreen;
                        _manager.ApplyChanges();
                    }
                }
            }

            _dirtyMatrix = true;

            BackBufferWidth = _manager.PreferredBackBufferWidth;
            BackBufferHeight = _manager.PreferredBackBufferHeight;
        }

        /// <summary>
        ///     Recreates the scale matrix values.
        /// </summary>
        private void RecreateScaleMatrix()
        {
            _dirtyMatrix = false;
            _scaleMatrix = Matrix.CreateScale(
                xScale: (float)_manager.GraphicsDevice.Viewport.Width / VirtualWidth,
                yScale: (float)_manager.GraphicsDevice.Viewport.Height / VirtualHeight,
                zScale: 1.0f);
        }

        /// <summary>
        ///     Sets the Viewport of the graphics device to (0, 0, 1, 1);
        /// </summary>
        public void FullViewport()
        {
            Viewport vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = BackBufferWidth;
            vp.Height = BackBufferHeight;
            _manager.GraphicsDevice.Viewport = vp;
        }

        public void ResetViewport()
        {
            float targetAspectRatio = VirtualAspectRatio;

            //  figure out the largest area that fits int his resolution at the desired aspect ratio
            int width = _manager.PreferredBackBufferWidth;
            int height = (int)(width / targetAspectRatio + 0.5f);
            bool changed = false;

            if (height > _manager.PreferredBackBufferHeight)
            {
                height = _manager.PreferredBackBufferHeight;

                //  Pillerbox
                width = (int)(height * targetAspectRatio + 0.5f);
                changed = true;
            }

            //  Setup the new viewport centered in the backbuffer
            Viewport viewport = new Viewport();

            viewport.X = (_manager.PreferredBackBufferWidth / 2) - (width / 2);
            viewport.Y = (_manager.PreferredBackBufferHeight / 2) - (height / 2);
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                _dirtyMatrix = true;
            }

            _manager.GraphicsDevice.Viewport = viewport;
        }

        /// <summary>
        ///     Shortcut method to setting a <see cref="RenderTarget2D"/> for the
        ///     GraphicsDevice to render to.
        /// </summary>
        /// <param name="target">
        ///     The <see cref="RenderTarget2D"/> to set.
        /// </param>
        public void SetRenderTarget(RenderTarget2D target)
        {
            _manager.GraphicsDevice.SetRenderTarget(target);
        }

        /// <summary>
        ///     Shortcut method to clear the GraphicsDevice buffer to the
        ///     color value of <see cref="ClearColor"/>
        /// </summary>
        public void Clear()
        {
            Clear(ClearColor);
        }

        /// <summary>
        ///     Shortcut method to clear the GraphicsDevice buffer to color
        ///     value given.
        /// </summary>
        /// <param name="color">
        ///     The color value to use when clearing.
        /// </param>
        public void Clear(Color color)
        {
            _manager.GraphicsDevice.Clear(color);
        }

    }
}
