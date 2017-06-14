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
            OnRaiseMenuStateChanged(new MenuStateChanged(1)); //Code for entering the options menu            
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseMenuStateChanged(new MenuStateChanged(-1)); //Code for closing the window
        }

        private void EnterGameOptions_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseMenuStateChanged(new MenuStateChanged(2));
        }
    }
}
