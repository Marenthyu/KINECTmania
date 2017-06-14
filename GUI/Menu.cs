using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.GUI
{
    interface Menu
    {
        event EventHandler<MenuStateChanged> RaiseMenuStateChanged;

        void OnRaiseMenuStateChanged(MenuStateChanged e);

    }
}
