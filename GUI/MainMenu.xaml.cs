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
    public partial class MainMenu : Page
    {
        GameWindow parent;
        public MainMenu(GameWindow parentWindow)
        {
            InitializeComponent();
            GameWindow parent = parentWindow;
        }

        private void EnterOptions_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Sollte es so gehen, werde ich alle Spiel-Menüs in WPF-Pages packen und diese via Buttons "verbinden"
             */
            OptionsMenu optionsMenu = new OptionsMenu();
            if (parent != null) {
                parent.Content = optionsMenu;
            }
            else
            {
                pa
                /*
                 * TODO: Diesen Code löschen. Hier soll stattdessen ein Event gefuert werden, welches
                 * das aktuelle GameWindow veranlasst, den Content auf eine Page zu wechseln. Dieses
                 * Event hat (codiert) die gewünschte Page zu enthalten.
                 */
            }
            
        }
    }
}
