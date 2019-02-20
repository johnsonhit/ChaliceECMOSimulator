using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChaliceECMOSimulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Timer modelingTimer;

        double rad = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }


        private void ModelCycle(object state)
        {
            // do some work not connected with UI
            rad += 0.1;
            if (rad > Math.PI * 2) rad = 0;

            graph1.UpdateGraphData((35 * Math.Cos(rad)) + 50, (20 * Math.Sin(rad)) + 50, (35 * Math.Tan(rad)) + 50, (20 * Math.Sin(rad)) + 50) ;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            graph1.GraphMaxY = 100;
            graph1.DataRefreshRate = 15;
            graph1.Graph2Enabled = true;
            graph1.Legend2 = "white";
            graph1.IsSideScrolling = false;
            graph1.GraphicsClearanceRate = 1000;
            graph1.RedrawGrid();
            modelingTimer = new Timer(ModelCycle, null, 0, 15);
        }
    }
}
