using KINECTmania.GameLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KINECTmania.GUI
{

    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static GameStateManager gms = new GameStateManager();

        internal static GameStateManager Gms { get => gms; set => gms = value; }
    }
}
