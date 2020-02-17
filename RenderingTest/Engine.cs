//-----------------------------------------------------------------------------
//  Rendering Test
//
//
//  This rendering test is designed to test the fuctionality of the Graphics
//  class and the RenderingPipeline class.
//
//  The values of _resolutionWidth, _resolutionHeight, _virtualWidth, and
//  _virtualHeight can be adjusted for the test below.
//
//  WHen running the test, you can pres the Up key on the keybaord to test rendreing
//  with the base resolution and the virtual resolution matching 1:1.
//
//  You can press the Down key on the keyboard to test rendering with the 
//  base resolution at half the virtual resolution.
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RenderingTest
{
    public class Engine : Game
    {
        //  Static reference to this engine instance.
        public static Engine Instance { get; private set; }

        //  Static reference to the graphics instance for manging and controlling
        //  the presentation of graphics.
        public static Graphics Graphics { get; set; }

        //  The renderers that will be rendered though the pipeline.
        private Renderer[] _renderers = new Renderer[1];

        //  Current and previous keyboard state for input.
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;


        //  The base width to apply to the backbuffer
        private int _resolutionWidth = 1280;
        
        //  The base height to apply to the backgbuffer
        private int _resolutionHeight = 720;

        //  The height of the virtual resolution
        private int _virtualWidth = 1280;

        //  The height of the virtual resolution
        private int _virtualHeight = 720;



        /// <summary>
        ///     Creates a new <see cref="Engine"/> instance.
        /// </summary>
        public Engine()
        {
            Instance = this;
            Graphics = new Graphics(this);

            //  Initialize the graphics with a backbuffer that is half the resolution width and height
            //  for testing.
            Graphics.Initialize(backbufferWidth: _resolutionWidth / 2,
                                backbufferHeight: _resolutionHeight / 2,
                                virtualWidth: _virtualWidth,
                                virtualHeight: _virtualHeight,
                                fullscreen: false,
                                onGraphicsCreated: OnGraphicsCreated,
                                onGraphicsReset: OnGraphicsReset,
                                onClientSizeChanged: null);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        ///     Called when the graphics device is created.  When this occurs, contents of
        ///     VRAM are wiped and thigns such as render targets need to be recreated.
        /// </summary>
        protected void OnGraphicsCreated()
        {
            if (RenderingPipeline.IsInitialized)
            {
                RenderingPipeline.OnGraphicsCreated();

                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].OnGraphicsCreated();
                }
            }
        }

        /// <summary>
        ///     Called when the graphics device is reset.  When this occurs, contents of
        ///     VRAM are wiped and thigns such as render targets need to be recreated.
        /// </summary>
        protected void OnGraphicsReset()
        {
            if (RenderingPipeline.IsInitialized)
            {
                RenderingPipeline.OnGraphicsReset();

                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].OnGraphicsReset();
                }
            }
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            RenderingPipeline.Initialize(Graphics.Device);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _renderers[0] = new Renderer(width: Graphics.VirtualWidth,
                                         height: Graphics.VirtualHeight,
                                         texture: Content.Load<Texture2D>(@"render_boundry_1280x720"));
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            RenderingPipeline.Unload();
            for(int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].Unload();
            }
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">
        ///     Provides a snapshot of timing values.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            //  Exit with escape
            if (_currentKeyboardState.IsKeyDown(Keys.Escape)) { Exit(); }

            //  If we press the Up key on the keyboard, then we set the resolution to match the virtual resolution
            //  otherwise if we press the Down key on the keyboard, we set the resolution to be half the virtual
            //  resolution.
            if(_currentKeyboardState.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Up))
            {
                Engine.Graphics.SetResolution(_resolutionWidth, _resolutionHeight, false);
            }
            else if(_currentKeyboardState.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyUp(Keys.Down))
            {
                Engine.Graphics.SetResolution(_resolutionWidth / 2, _resolutionHeight / 2, false);
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">
        ///     Provides a snapshot of timing values.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            RenderingPipeline.Render(_renderers);

            Window.Title = $"Base Resolution: {Engine.Graphics.BackBufferWidth}x{Engine.Graphics.BackBufferHeight} -- Virtual Resolution: {Engine.Graphics.VirtualWidth}x{Engine.Graphics.VirtualHeight}";

            base.Draw(gameTime);
        }
    }
}
