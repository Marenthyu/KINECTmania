using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.GUI
{
    public class NotifyGameWindowIsBuilt : EventArgs
    {
        private bool paradoxIstGruenUndHaesslich;

        public bool ParadoxIstGruenUndHaesslich
        {
            get { return paradoxIstGruenUndHaesslich; }
            set { paradoxIstGruenUndHaesslich = value; }
        }

        public NotifyGameWindowIsBuilt(bool b)
        {
            paradoxIstGruenUndHaesslich = b;
        }
    }
}
