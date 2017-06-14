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
    /*
     * Songfile-Einlesen: In FileParser.cs. Dafür muss ein Event gefeuert werden. Default-Ordner festlegen!
     */
    /// <summary>
    /// Interaktionslogik für GameOptionsMenu.xaml
    /// </summary>
    public partial class GameOptionsMenu : Page, Menu
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        public GameOptionsMenu()
        {
            InitializeComponent();
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }
    }
}
