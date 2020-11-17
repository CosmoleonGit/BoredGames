using Microsoft.Xna.Framework;
using MonoExt;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Source.MainMenu
{
    public abstract class MenuScreen : ControlGroup
    {
        //protected Main main;


#if DEBUG
        static readonly string versionStr = Settings.GetVersion + " (Indev)";
#else
        static readonly string versionStr = Settings.GetVersion;
#endif

        const string signature = "Cosmoleon 2020";
        public MenuScreen()
        {
            //main = _main;
            
            components.Add(new Label(this)
            {
                Position = new Point(0, Main.screenHeight),
                Font = Main.MediumFont,
                Text = versionStr,
                TextAlign = Label.TextAlignEnum.BOTTOM_LEFT
            });

            components.Add(new Label(this)
            {
                Position = new Point(Main.screenWidth, Main.screenHeight),
                Font = Main.MediumFont,
                Text = signature,
                TextAlign = Label.TextAlignEnum.BOTTOM_RIGHT
            });
        }
    }
}
