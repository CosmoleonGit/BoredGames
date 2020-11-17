using BoardGames.Source.FunScreen;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.MainMenu
{
    public class WaitingScreen : MenuScreen, ICanReceive
    {
        static WaitingScreen()
        {
            Explosion.LoadContent(Main.main.Content);
        }
        
        /*
        public static void LoadContent(ContentManager content)
        {
            Explosion.LoadContent(content);
        }
        */

        Explosion explosion;

        readonly Label label;
        readonly Button button;

        public WaitingScreen(string text)
        {
            label = new Label(this)
            {
                Position = new Point(Main.screenWidth / 2, Main.screenHeight / 2),
                Font = Main.MediumFont,
                Text = text,
                TextAlign = Label.TextAlignEnum.CENTRE_CENTRE
            };

            button = new Button(this)
            {
                Font = Main.MediumFont,
                Text = "Cancel",
                Size = new Point(100, 50)
            };

            button.ClickEvent += (object s) =>
            {
                Networking.Stop();
                Main.main.Screen = new ChooseScreen();
            };

            components.Add(label);
            components.Add(button);

            Networking.OnConnect += () =>
            {
                var msg = Networking.CreateMessage();
                msg.Write(Properties.Settings.Default.username);
                Networking.SendMessage(msg);

                /*
                button.active = false;
                label.Text = "You are now connected to " + PlayerConnection.connection.username + "!";
                explosion = new Explosion();
                explosion.Finished += () =>
                {
                    animDone = true;
                    //main.screen = new MainScreen();
                };
                */
            };

            
        }

        bool animDone;
        int delay = 60;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (animDone)
            {
                delay--;
                if (delay == 0) Main.main.Screen = new MainScreen();
            }
            else if (explosion != null)
            {
                explosion.Update(gameTime);
            }
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (explosion != null)
            {
                explosion.Show(gameTime, spriteBatch);
            }

            base.Show(gameTime, spriteBatch);
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            Settings.username = msg.ReadString();

            button.active = false;
            label.Text = "You are now connected to " + Settings.username + "!";
            explosion = new Explosion();
            explosion.Finished += () =>
            {
                animDone = true;
                //main.screen = new MainScreen();
            };
        }

        private class Explosion : Component
        {
            static SoundEffect sound;
            static Texture2D particleTex;

            public static void LoadContent(ContentManager content)
            {
                particleTex = content.Load<Texture2D>("Circle");
                sound = content.Load<SoundEffect>("Connected");
            }

            const int particleCount = 100;
            public Explosion()
            {
                sound.Play();

                for (int i = 0; i < particleCount; i++)
                {
                    particles.Add(new Particle());
                }
            }

            readonly List<Particle> particles = new List<Particle>();

            public override void Update(GameTime gameTime)
            {
                particles.ForEach(x => x.Update(gameTime));
                particles.RemoveAll(x => x.Finished);

                if (particles.Count == 0) Finished.Invoke();
            }

            public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                particles.ForEach(x => x.Show(gameTime, spriteBatch));
                spriteBatch.End();
            }

            public event EmptyDel Finished;

            private class Particle : Component
            {
                public Particle()
                {
                    float r = (float)(Main.random.NextDouble() * MathHelper.TwoPi);

                    velocity = new Vector2((float)Math.Cos(r), (float)Math.Sin(r));
                    
                    velocity *= (float)Main.random.NextDouble() * 3 + 1;

                    switch (Main.random.Next(7))
                    {
                        default:
                            c = Color.Red;
                            break;
                        case 1:
                            c = Color.Orange;
                            break;
                        case 2:
                            c = Color.Yellow;
                            break;
                        case 3:
                            c = Color.Lime;
                            break;
                        case 4:
                            c = Color.Cyan;
                            break;
                        case 5:
                            c = Color.Blue;
                            break;
                        case 6:
                            c = Color.Purple;
                            break;
                    }

                }

                Vector2 position;
                Vector2 velocity;

                Color c;

                static readonly Vector2 scale = new Vector2(16);

                int timeLeft = Main.random.Next(75, 125);

                public bool Finished => timeLeft == 0;

                public override void Update(GameTime gameTime)
                {
                    position += velocity;
                    timeLeft--;
                }

                const int fadeLength = 10;

                public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
                {
                    Color newC;

                    if (timeLeft > fadeLength)
                    {
                        newC = c;
                    } else
                    {
                        newC = Color.Lerp(c, Color.Transparent, (float)timeLeft / -fadeLength + 1);
                    }

                    spriteBatch.Draw(particleTex, new Rectangle((position - scale / 2).ToPoint() + new Point(Main.screenWidth / 2, Main.screenHeight / 2), scale.ToPoint()), newC);
                }
            }
        }
    }
}
