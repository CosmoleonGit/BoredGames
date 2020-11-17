using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using MonoNet;

namespace BoardGames.Source.MainMenu
{
    public class HostScreen : MenuScreen
    {
        static string ipStr = "";

        static string GetIP()
        {
            if (ipStr == "")
            {
                try
                {
                    ipStr = new WebClient().DownloadString("https://ipv4.icanhazip.com/").TrimEnd();
                }
                catch { }
            }

            return ipStr;
        }

        public HostScreen()
        {
            
            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2 - 25, Main.screenHeight / 2 - 25),
                Font = Main.MediumFont,
                Text = "Your IP:",
                TextAlign = Label.TextAlignEnum.CENTRE_RIGHT
            });
            

            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth / 2 - 25, Main.screenHeight / 2 + 25),
                Font = Main.MediumFont,
                Text = "Port:",
                TextAlign = Label.TextAlignEnum.CENTRE_RIGHT
            });


            TextBox ipTxt = new TextBox(this, Main.main.Window)
            {
                Position = new Point(Main.screenWidth / 2 + 25, Main.screenHeight / 2 - 35),
                Size = new Point(100, 20),
                charLimit = 15,
                Font = Main.SmallFont,

#if DEBUG
                Text = "127.0.0.1"
#else
                Text = GetIP()
#endif

            };
            

            TextBox portTxt = new TextBox(this, Main.main.Window)
            {
                Position = new Point(Main.screenWidth / 2 + 25, Main.screenHeight / 2 + 15),
                Size = new Point(100, 20),
                charLimit = 4,
                Font = Main.SmallFont

#if DEBUG
                ,
                Text = "1234"
#endif
            };
            
            Button beginBtn = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50 - 75, Main.screenHeight / 2 + 25 + 50),
                Size = new Point(100, 50),
                Font = Main.MediumFont,
                Text = "Begin Host"
            };

            beginBtn.ClickEvent += (object s) =>
            {
                void Error()
                {
                    Main.main.Screen = new OKScreen("Invalid input.", new Action(() => Main.main.Screen = new HostScreen()));
                }

                string ConvertToStr(int num, int len)
                {
                    string ret = "";

                    if (len > 0)
                    {
                        
                        for (int i = 1; i < len; i++)
                        {
                            ret = ret.Insert(0, ((char)((num / Math.Pow(26, i)) % 26 + 65)).ToString());
                        }

                        ret += (char)(num % 26 + 65);
                    }

                    return ret;
                    
                }

                string[] ipParts = ipTxt.Text.Split('.');

                if (ipParts.Length != 4) { Error(); return; }

                string code = "";
                for (int i = 0; i < 4; i++)
                {
                    if (!int.TryParse(ipParts[i], out int ip)) { Error(); return; }
                    code += ConvertToStr(ip, 2);
                }

                if (!int.TryParse(portTxt.Text, out int port)) { Error(); return; }
                code += ConvertToStr(port, 4);

                Main.main.Screen = new WaitingScreen("Your code is: " + code + Environment.NewLine + Environment.NewLine + "Waiting for someone to connect...");

                Networking.SetupServer(port);
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

            components.Add(beginBtn);
            components.Add(cancelBtn);

            components.Add(ipTxt);
            components.Add(portTxt);
        }

        
    }
}
