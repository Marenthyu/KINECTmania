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
    /// Interaktionslogik für Page2.xaml
    /// </summary>
    public partial class OptionsMenu : Page, Menu
    {

        #region <Event handling>
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        public OptionsMenu()
        {
            InitializeComponent();
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        #endregion

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseMenuStateChanged(new MenuStateChanged(0));
        }
    }
}
