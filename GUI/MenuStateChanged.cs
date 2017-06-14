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
            menuState = b;
        }
    }
}
