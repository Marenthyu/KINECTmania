﻿using System;
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

        public Menu[] Menus{
            get { return menus; }
        }
        public GameWindow()
        {
            InitializeComponent();

            menus = new Menu[4];
            GameOptionsMenu gom = new GameOptionsMenu();
            GamePage gp = new GamePage();
            menus[0] = new MainMenu();
            menus[1] = new OptionsMenu();
            menus[2] = gom;
            menus[3] = gp; //- dieses Menü wurde noch nicht vollständig implementiert!

            this.Content = menus[0];

            //Set Publishers
            foreach(Menu publisher in menus)
            {
                if (publisher != null)
                {
                    publisher.RaiseMenuStateChanged += HandleMenuStateChanged;
                }
            }
            gp.setPublisherMenuStateChanged(gom);
            
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
                        Console.WriteLine("Info: menus[3] isn't implemented yet so you won't be able to access it either. Logical?");
                    }
                    break;
                case -1:
                    this.Close();
                    break;
                default:
                    this.Content = menus[0];
                    Console.WriteLine("Warning: We recieved an invalid MenuState (" + e.MenuState + ") from the object " + sender.ToString() + "! Defaulting to menus[0] (" + menus[0].ToString() + ")...");
                    break;
            }
        }
        
        

    }
    
}
