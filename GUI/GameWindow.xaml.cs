using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KINECTmania.GUI
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public event EventHandler<NotifyGameWindowIsBuilt> seeType;
        public GameWindow()
        {
            InitializeComponent();
            this.Content = new MainMenu(this);
        }

        public virtual void OnRaiseNotifyGameWindowBuilt(NotifyGameWindowIsBuilt e)
        {
            EventHandler<NotifyGameWindowIsBuilt> handler = seeType;
            if(handler != null)
            {
                e.ParadoxIstGruenUndHaesslich = true;
                handler(this, e);
            }
        }
        

    }
    
}
