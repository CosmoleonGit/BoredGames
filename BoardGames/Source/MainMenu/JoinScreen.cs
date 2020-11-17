using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using MonoNet;

namespace BoardGames.Source.MainMenu
{
    public class JoinScreen : MenuScreen
    {
        public JoinScreen()
        {
            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2 - 25, Main.screenHeight / 2 - 25),
                Font = Main.MediumFont,
                Text = "Code:",
                TextAlign = Label.TextAlignEnum.CENTRE_RIGHT
            });

            TextBox codeTxt = new TextBox(this, Main.main.Window)
            {
                Position = new Point(Main.screenWidth / 2 + 25, Main.screenHeight / 2 - 35),
                Size = new Point(100, 20),
                charLimit = 15,
                Font = Main.SmallFont,

#if DEBUG
                Text = "EXAAAAABABVM"
#else
                Text = Properties.Settings.Default.code
#endif
            };

            Button connectBtn = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50 - 75, Main.screenHeight / 2 + 25 + 50),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Connect"
            };

            Button cancelBtn = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50 + 75, Main.screenHeight / 2 + 25 + 50),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Cancel"
            };

            cancelBtn.ClickEvent += (object s) =>
            {
                Main.main.Screen = new ChooseScreen();
            };

            connectBtn.ClickEvent += (object s) =>
            {
                void Error()
                {
                    Main.main.Screen = new OKScreen("Invalid input.", new Action(() => Main.main.Screen = new JoinScreen()));
                }

                bool tryStrToInt(string str, out int val, int max = int.MaxValue)
                {
                    val = 0;

                    for (int i = str.Length - 1; i >= 0; i--)
                    {
                        int j = str.Length - i;

                        int v = (str[i]) - 65;
                        if (v < 0 && v > 25) return false;

                        val += v * (int)Math.Pow(26, j - 1);
                    }

                    if (val > max) return false;

                    return true;
                }

                if (codeTxt.Text.Length != 12) { Error(); return; }

                if (!tryStrToInt(codeTxt.Text.Substring(0, 2), out int p1, 255) ||
                    !tryStrToInt(codeTxt.Text.Substring(2, 2), out int p2, 255) ||
                    !tryStrToInt(codeTxt.Text.Substring(4, 2), out int p3, 255) ||
                    !tryStrToInt(codeTxt.Text.Substring(6, 2), out int p4, 255) ||
                    !tryStrToInt(codeTxt.Text.Substring(8, 4), out int p5, 65535))
                {
                    Error(); return;
                }

                string ip = p1 + "." + p2 + "." + p3 + "." + p4;
                //Console.WriteLine(ip + ":" + p5);

                Properties.Settings.Default.code = codeTxt.Text;
                Properties.Settings.Default.Save();

                Main.main.Screen = new WaitingScreen("Waiting to connect...");

                Networking.SetupClient(ip, p5);
            };

            components.Add(connectBtn);
            components.Add(cancelBtn);
            components.Add(codeTxt);
        }
    }
}
