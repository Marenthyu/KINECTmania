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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KINECTmania.GUI
{
    /// <summary>
    /// Interaktionslogik für MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page, Menu
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        public MainMenu()
        {
            InitializeComponent();
        }

        private void EnterOptions_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseMenuStateChanged(new MenuStateChanged(1));
                /*
                 * TODO: Diesen Code löschen. Hier soll stattdessen ein Event gefuert werden, welches
                 * das aktuelle GameWindow veranlasst, den Content auf eine Page zu wechseln. Dieses
                 * Event hat (codiert) die gewünschte Page zu enthalten.
                 */
        
            
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

    }
}
