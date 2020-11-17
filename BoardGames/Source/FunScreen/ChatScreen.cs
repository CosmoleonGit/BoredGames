using BoardGames.Source.MainMenu;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.FunScreen
{
    internal class ChatScreen : ControlGroup, ICanReceive
    {
        static SoundEffect messageSound;
        static ChatScreen()
        {
            messageSound = Main.main.Content.Load<SoundEffect>("Message");
        }

        const int left = 512;

        static readonly Rectangle r = new Rectangle(0, 0, Main.screenWidth - left, Main.screenHeight);

        readonly List<string> messages = new List<string>();
        //Queue<string> messages = new Queue<string>();

        int scroll = 0;
        const int messageScreenLimit = 22;

        static readonly Color barColour = Color.Black;
        static readonly Color backColour = new Color(0, 15, 25);

        const int barHeight = 50;

        //static readonly Matrix matrix = Matrix.CreateTranslation(left, 0, 0);

        public ChatScreen()
        {
            Matrix = Matrix.CreateTranslation(left, 0, 0);

            const int boxHeight = 50;
            const int btnWidth = 50;

            components.Add(new Label(this)
            {
                Position = new Point(10, barHeight / 2),
                Font = Main.MediumFont,
                Text = Settings.username,
                TextAlign = Label.TextAlignEnum.CENTRE_LEFT
            });

            Button disconnectBtn = new Button(this)
            {
                Position = new Point(r.Right - 100, r.Y),
                Size = new Point(100, barHeight),
                Font = Main.MediumFont,
                Text = "Disconnect"
            };

            disconnectBtn.ClickEvent += (object s) =>
            {
                Main.main.Screen = new ChooseScreen();
                Networking.Stop();
            };

            TextBox inputTxt = new TextBox(this, Main.main.Window)
            {
                Position = new Point(0, Main.screenHeight - boxHeight),
                Size = new Point(r.Width - btnWidth, boxHeight),
                Font = Main.SmallFont,
            };

            Button sendBtn = new Button(this)
            {
                Font = Main.MediumFont,
                Text = "Send",
                Position = new Point(r.Right - btnWidth, Main.screenHeight - boxHeight),
                Size = new Point(btnWidth, boxHeight)
            };

            inputTxt.EnterPressed += () => sendBtn.PerformClick();

            sendBtn.ClickEvent += (object s) =>
            {
                if (inputTxt.Text != "")
                {
                    ShowMessage("(" + Properties.Settings.Default.username + "): " + inputTxt.Text);
                    var msg = Networking.CreateMessage();

                    msg.Write((byte)MainScreen.Side.CHAT_SIDE);
                    msg.Write(inputTxt.Text);
                    Networking.SendMessage(msg);
                    inputTxt.Text = "";
                }
            };

            components.Add(disconnectBtn);
            components.Add(sendBtn);
            components.Add(inputTxt);
        }

        private void ShowMessage(string s)
        {
            //messages.Add(s);
            //messages.Enqueue(s);
            foreach (string str in WrapText(Main.SmallFont, s, r.Width - 20).Split(new char[] { '\r', '\n' }))
            {
                messages.Add(str);
            }
        }

        const int spacing = 20;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int delta = Input.GetScrollDelta();

            if (delta > 0)
            {
                if (scroll != messages.Count - messageScreenLimit) scroll++;
            } else if (delta < 0)
            {
                if (scroll != 0) scroll--;
            }
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix:Matrix);

            spriteBatch.Draw(SpecialContent.Pixel, new Rectangle(0, 0, Main.screenWidth - left, Main.screenHeight), backColour);
            spriteBatch.Draw(SpecialContent.Pixel, new Rectangle(0, 0, Main.screenWidth - left, barHeight), backColour);

            int i = 0;
            for (int j = Math.Max(0, messages.Count - messageScreenLimit) - scroll; j < messages.Count - scroll; j++)
            {
                i++;
                spriteBatch.DrawString(Main.SmallFont, messages[j], new Vector2(10, i * spacing + barHeight), Color.White);
            }

            spriteBatch.End();

            base.Show(gameTime, spriteBatch);
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            ShowMessage("(" + Settings.username + "): " + msg.ReadString());
            messageSound.Play();
        }

        string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (sb.ToString() == "")
                        {
                            sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                        else
                        {
                            sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
