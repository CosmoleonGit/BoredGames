using BoardGames.Source;
using BoardGames.Source.FunScreen;
using BoardGames.Source.Games;
using BoardGames.Source.Games.BlockFall;
using BoardGames.Source.Games.TurnGames;
using BoardGames.Source.Games.TurnGames.Chess;
using BoardGames.Source.Games.TurnGames.Draughts;
using BoardGames.Source.MainMenu;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;

namespace BoardGames
{
    public class Main : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        
        public const int screenWidth = 768,
                         screenHeight = 562;
        
        Component s;

        public Component Screen { 
            get => s; 
            set 
            {
                //if (s is IDisposable sid) sid.Dispose();
                s = value;
            } }
        
        public static SpriteFont SmallFont { get; private set; }
        public static SpriteFont MediumFont { get; private set; }

        public static Random random = new Random();

        public static Main main;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = screenWidth,
                PreferredBackBufferHeight = screenHeight,
                PreferMultiSampling = true,
                IsFullScreen = false
            };

            Window.Title = "Bored Games";
            
            Content.RootDirectory = "Content";

            main = this;
        }

        protected override void Initialize()
        {
            //Properties.Settings.Default.Reset();
            IsMouseVisible = false;
            //IsFixedTimeStep = false;

#if !DEBUG
            var logoScreen = new SplashScreen(Content, screenWidth, screenHeight, delegate ()
            {
                if (Properties.Settings.Default.username != "")
                {
                    Screen = new PlayScreen();
                }
                else
                {
                    Screen = new UsernameScreen(new Action(() => { Screen = new PlayScreen(); }));
                }
            });

            Screen = logoScreen;
#endif

            Networking.AppName = $"BoardGames{Settings.GetVersion}";

            Networking.OnError += (string s) =>
            {
                Screen = new OKScreen("An error has occured." + Environment.NewLine + Environment.NewLine + s, new Action(() => Screen = new ChooseScreen()));
            };

            Networking.OnDisconnect += (string s) =>
            {
                Screen = new OKScreen(s, new Action(() => Screen = new ChooseScreen()));
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SmallFont = Content.Load<SpriteFont>("Fonts/SmallFont");
            MediumFont = Content.Load<SpriteFont>("Fonts/MediumFont");

            SpecialContent.LoadContent(GraphicsDevice);

            Input.LoadData(this);

#if DEBUG
            Properties.Settings.Default.username = "Debug";
            Screen = new ChooseScreen();
            //Screen = new TetrisGame(new MainScreen());
#endif
        }

        protected override void UnloadContent()
        {
            SpecialContent.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            Networking.Update();

            if (Networking.Connected)
            {
                NetIncomingMessage msg;
                while ((msg = Networking.ReceiveMessage()) != null)
                {
                    if (Screen is ICanReceive cr) cr.ReceiveMessage(msg);
                }
            }

            Screen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Screen.Show(gameTime, spriteBatch);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Input.DrawCursor(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Networking.Stop();

            base.OnExiting(sender, args);
        }

        
    }
}
