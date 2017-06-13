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

        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /**
             *  Statt an der main-Methode herumzubasteln, wäre es besser, die Initialisierung des Programms (Kinect-Sensor, Gamelogik, ...) hier durchzuführen
             */
        }
    }
}
