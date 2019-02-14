using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ChaliceECMOSimulator
{
    public sealed partial class SingleParameterController : UserControl
    {

        public SingleParametersViewModel ViewModel { get; set; }

        public SingleParameterController()
        {
            this.InitializeComponent();
            this.ViewModel = new SingleParametersViewModel();
        }       
    }
}
