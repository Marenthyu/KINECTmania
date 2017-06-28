using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.GUI
{
    public class MenuStateChanged : EventArgs
    {
        private int menuState;

        public int MenuState
        {
            get { return menuState; }
            set { menuState = value; }
        }

        public MenuStateChanged(int b)
        {
            ///<summary>
            /// 0: main menu
            /// 1: options menu (changing volume, ...)
            /// 2: game settings menu (changing bpm, song, ... just before start of the game)
            /// 3: during game
            /// -1: exiting the game window
            ///</summary>
            menuState = b;
        }
    }
}
