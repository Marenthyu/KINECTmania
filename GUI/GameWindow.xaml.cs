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
        private Menu[] menus;
        public GameWindow()
        {
            InitializeComponent();

            menus = new Menu[4];
            menus[0] = new MainMenu();
            menus[1] = new OptionsMenu();
            menus[2] = new GameOptionsMenu();
            //menus[3] = new GameMenu();

            this.Content = menus[0];

            foreach(Menu publisher in menus)
            {
                publisher.RaiseMenuStateChanged += HandleMenuStateChanged;
            }
            
        }


        void HandleMenuStateChanged(object sender, MenuStateChanged e)
        {
            switch (e.MenuState)
            {
                case 0:
                    this.Content = menus[0];
                    break;
                case 1:
                    this.Content = menus[1];
                    break;
                case 2:
                    this.Content = menus[2];
                    break;
                case 3:
                    if (menus[3] != null)
                    {
                        this.Content = menus[3];
                    }
                    else
                    {
                        Console.WriteLine("Info: menus[3] isn't implemented yet so you won't be able to access it too. Logical?");
                    }
                    break;
                default:
                    this.Content = menus[0];
                    Console.WriteLine("Warning: We recieved an invalid MenuState (" + e.MenuState + ") from the object " + sender.ToString() + "! Defaulting to menus[0] (" + menus[0].ToString() + ")...");
                    break;
            }
        }
        
        

    }
    
}
