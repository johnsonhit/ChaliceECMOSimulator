using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class SingleParameterController : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public double CurrentValue { get; set; } = 145;
        public double CurrentValue2 { get; set; } = 85;
        public int ControllerState { get; set; } = 0;       // 0 = idle, 1 = running, 2 = waiting, 3 = armed

        public double Incrementor { get; set; } = 1;
        double deltaValue = 0;
        double timeIn = 0;
        double timeAt = 0;
        double _maxValuePgb = 100;
        public double MaxValuePgb { get { return _maxValuePgb; } set { _maxValuePgb = value; OnPropertyChanged(); } }

        int _controlHeight = 352;
        public int ControlHeight { get { return _controlHeight; } set { _controlHeight = value; OnPropertyChanged(); } }

        double MaxTimeAtValue = 0;
        double MaxTimeInValue = 0;

        int expandedState = 1;  // 0 = small, 1 = expanded, 2 = settings
        bool blinker = true;
        bool initialized = false;
        bool running = false;

        bool _dualControllerMode = false;
        bool DualControllerMode { get { return _dualControllerMode; } set { _dualControllerMode = value; OnPropertyChanged(); } }

        bool _fluctStatus = false;
        public bool FluctStatus { get { return _fluctStatus; } set { _fluctStatus = value; OnPropertyChanged(); } }
        string _fluctStatusString = "off";
        public string FluctStatusString { get { return _fluctStatusString; } set { _fluctStatusString = value; OnPropertyChanged(); } }

        bool _fluctMode = false;
        public bool FluctMode { get { return _fluctMode; }
            set {
                _fluctMode = value;
                if (value)
                {
                    FluctModeString = "random";
                } else
                {
                    FluctModeString = "sinus";
                }
                OnPropertyChanged(); }
        }

        string _fluctModeString = "sinus";
        public string FluctModeString {
            get { return _fluctModeString; }
            set { 
                _fluctModeString = value;
 
                OnPropertyChanged(); }
        }

        double fluctWidth = 0;
        double fluctTime = 0;

        private double _timeLeft = 0;
        public double TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(); } }

        private string _currentValueString = "145";
        public string CurrentValueString
        {
            get
            {
                return _currentValueString;
            }
            set
            {
                _currentValueString = value;
                OnPropertyChanged();
            }
        }

        private string _title = "HEARTRATE";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private double _targetValue = 145;
        public double TargetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                _targetValue = value;
                OnPropertyChanged();
            }
        }

        private double _targetValue2 = 85;
        public double TargetValue2
        {
            get
            {
                return _targetValue2;
            }
            set
            {
                _targetValue2 = value;
                OnPropertyChanged();
            }
        }

        private double _upperLimit = 280;
        public double UpperLimit
        {
            get { return _upperLimit; }
            set
            {
                _upperLimit = value;
                OnPropertyChanged();
            }
        }

        private double _lowerLimit = 0;
        public double LowerLimit
        {
            get { return _lowerLimit; }
            set
            {
                _lowerLimit = value;
                OnPropertyChanged();
            }
        }

        private string _controllerStatus = "START";
        public string ControllerStatus
        {
            get { return _controllerStatus; }
            set { _controllerStatus = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _statusColor = new SolidColorBrush(Colors.DarkGray);
        public SolidColorBrush StatusColor { get { return _statusColor; } set { _statusColor = value; OnPropertyChanged(); } }
        private SolidColorBrush _statusColorText = new SolidColorBrush(Colors.White);
        public SolidColorBrush StatusColorText { get { return _statusColorText; } set { _statusColorText = value; OnPropertyChanged(); } }

        private readonly Windows.UI.Xaml.DispatcherTimer updateTimer = new Windows.UI.Xaml.DispatcherTimer();

        public SingleParameterController()
        {
            this.InitializeComponent();

            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            initialized = true;

        }

        private void ButStart_Click(object sender, RoutedEventArgs e)
        {
            StartButtonPressed();
        }
        void StartButtonPressed()
        {
            if (running)
            {
                ResetToDirectControl();
            } else
            {
                CalculateAutomation();
                running = true;
            }

        }
        private void CmbAtTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtTimeChanged();
        }

        void AtTimeChanged()
        {
            timeAt = FindPeriod(cmbAtTime.SelectedIndex);
        }
        private void CmdInTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InTimeChanged();
        }
        void InTimeChanged()
        {
            timeIn = FindPeriod(cmdInTime.SelectedIndex);
        }

        double FindPeriod(int selected)
        {
            double period = 0;
            switch (selected)
            {
                case 0:
                    period = 0;
                    break;
                case 1:
                    period = 5;
                    break;
                case 2:
                    period = 10;
                    break;
                case 3:
                    period = 20;
                    break;
                case 4:
                    period = 30;
                    break;
                case 5:
                    period = 60;
                    break;
                case 6:
                    period = 90; 
                    break;
                case 7:
                    period = 120;
                    break;
                case 8:
                    period = 180;
                    break;
                case 9:
                    period = 300;
                    break;
                case 10:
                    period = 600;
                    break;
                case 11:
                    period = 900;
                    break;
                default:
                    period = 0;
                    break;


            }
            return period;
        }
        private void UpdateTimer_Tick(object sender, object e)
        {
            blinker = !blinker;

          
            switch (ControllerState)
            {
                case 0:
                    ControllerStatus = "DIRECT CONTROL";
                    TimeLeft = 0;
                    StatusColor = new SolidColorBrush(Color.FromArgb(255,80,80,80));
                    StatusColorText = new SolidColorBrush(Colors.White);

                    break;
                case 1:
                    ControllerStatus = "RUNNING";
                    MaxValuePgb = MaxTimeInValue - 1;
                    StatusColor = new SolidColorBrush(Colors.DarkRed);
                    if (blinker) { StatusColorText = StatusColor; } else { StatusColorText = new SolidColorBrush(Colors.White); }
                   
                    if (timeIn > 0)
                    {
                        TimeLeft = timeIn;
                    }
                    break;
                case 2:
                    ControllerStatus = "WAITING";
                    MaxValuePgb = MaxTimeAtValue - 1;
                    StatusColor = new SolidColorBrush(Colors.DarkRed);
                    if (blinker) { StatusColorText = StatusColor; } else { StatusColorText = new SolidColorBrush(Colors.White); }
                    if (timeAt > 0)
                    {
                        TimeLeft = timeAt;
                    }
                    break;
                case 3:
                    ControllerStatus = "ARMED";
                    StatusColor = new SolidColorBrush(Colors.DarkOrange);
                    if (blinker) { StatusColorText = StatusColor; } else { StatusColorText = new SolidColorBrush(Colors.White); }
                    break;
            }

            if (blinker)
            {
                if (DualControllerMode)
                {
                    CurrentValueString = Math.Round(CurrentValue, 0).ToString() + "/" + Math.Round(CurrentValue2,0).ToString();
                } else
                {
                    CurrentValueString = Math.Round(CurrentValue, 0).ToString();
                }
               
            }

            UpdateParameters();

        }

        private void UpdateParameters()
        {
            if (running)
            {
                if (timeAt <= 0)
                {
                    if (deltaValue != 0)
                    {
                        timeAt = 0;
                        ControllerState = 1;
                        CurrentValue += deltaValue;
                        if (blinker) timeIn--;
                        if (Math.Abs(CurrentValue - TargetValue) < Math.Abs(deltaValue))
                        {
                            CurrentValue = TargetValue;
                            ResetToDirectControl();
                        }
                    }
                }
                else
                {
                    ControllerState = 2;
                    if (blinker) timeAt--;
                }
            }
      

        }

        public void CalculateAutomation()
        {
            if ( Math.Abs(TargetValue - CurrentValue) > 0)
            {
                MaxTimeAtValue = timeAt;
                MaxTimeInValue = timeIn;

                if (timeAt > 0)
                {
                    TimeLeft = MaxTimeAtValue;
                    MaxValuePgb = MaxTimeAtValue - 1;      
                }
                else
                {
                    TimeLeft = MaxTimeInValue - 1;
                    MaxValuePgb = MaxTimeInValue - 1;
                }

                if (timeIn > 0)
                {
                    deltaValue = ((TargetValue - CurrentValue) / timeIn) / 2;
                }
                else
                {
                    deltaValue = (TargetValue - CurrentValue);
                }

            } else
            {
                ControllerState = 0;
                timeAt = 0;
                timeIn = 0;
            }


        }

        private void SliTargetValue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (initialized)
            {
                ResetAutomation();
            }
        }

        private void SliTargetValue_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (initialized)
            {
                ResetAutomation();
            }
        }

        void ResetToDirectControl()
        {
            ControllerState = 0;
            timeIn = 0;
            deltaValue = 0;
            sliTargetValue.Value = Math.Round(CurrentValue,0);
            cmbAtTime.SelectedIndex = 0;
            cmdInTime.SelectedIndex = 0;
        
        }
        void ResetAutomation()
        {
            if (running)
            {
                ResetToDirectControl();

            } else
            {

                if (timeAt == 0 && timeIn == 0)
                {
                    CurrentValue = TargetValue;
                }


                if (TargetValue != CurrentValue)
                {
                    ControllerState = 3;
                }
                else
                {
                    ControllerState = 0;

                }

            }
        

            running = false;
        }

        private void ButPlus_Click(object sender, RoutedEventArgs e)
        {
            if (TargetValue + Incrementor <= UpperLimit) TargetValue += Incrementor;
        }

        private void ButMinus_Click(object sender, RoutedEventArgs e)
        {
            if (TargetValue - Incrementor >= LowerLimit) TargetValue -= Incrementor;
        }

        private void ButExpand_Click(object sender, RoutedEventArgs e)
        {
            
            if (expandedState == 0 || expandedState == 3)
            {
                expandedState = 1;
                ControlHeight = 352;
            } else
            {
                expandedState = 0;
                ControlHeight = 72;
            }
        }

        private void ButLimits_Click(object sender, RoutedEventArgs e)
        {
            if (expandedState == 0 || expandedState == 1)
            {
                expandedState = 3;
                ControlHeight = 450;
            }
            else
            {
                expandedState = 1;
                ControlHeight = 352;
            }
            butLimits.IsChecked = FluctStatus;
        }

        private void ButFluct_Click(object sender, RoutedEventArgs e)
        {
            FluctStatus = !FluctStatus;
            if (FluctStatus)
            {
                FluctStatusString = "on";
                bool test1 = (double.TryParse(txtFluctWidth.Text, out fluctWidth));
                bool test2 = (double.TryParse(txtFluctTime.Text, out fluctTime));

                if (test1 == false || test2 == false)
                {
                    FluctStatus = false;
                    FluctStatusString = "fail";
                }

            } else
            {
                FluctStatusString = "off";
            }
            
        }

        private void TxtFluctWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            FluctStatus = false;
            FluctStatusString = "off";
        }

        private void TxtFluctTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            FluctStatus = false;
            FluctStatusString = "off";
        }

        private void ButFluctMode_Click(object sender, RoutedEventArgs e)
        {
            FluctMode = !FluctMode;
        }

        private void ButAuto_Click(object sender, RoutedEventArgs e)
        {
            DualControllerModeToggle();
        }

        private void ButModel_Click(object sender, RoutedEventArgs e)
        {

        }

        public void DualControllerModeToggle()
        {
            DualControllerMode = !DualControllerMode;

            if (DualControllerMode)
            {
                // dual controller mode
                butLock.Visibility = Visibility.Visible;
                sliTargetValue2.Visibility = Visibility.Visible;
                txtTargetValue2.Visibility = Visibility.Visible;
                sliTargetValue.Margin = new Thickness(-30, 5, 0, 0);
                sliTargetValue2.Margin = new Thickness(30, 5, 0, 0);

                txtTargetValue.Margin = new Thickness(-30, 0, 0, 0);
                txtTargetValue2.Margin = new Thickness(30, 0, 0, 0);
            } else
            {
                butLock.Visibility = Visibility.Collapsed;
                sliTargetValue2.Visibility = Visibility.Collapsed;
                txtTargetValue2.Visibility = Visibility.Collapsed;

                sliTargetValue.Margin = new Thickness(0, 5, 0, 0);
                sliTargetValue2.Margin = new Thickness(0, 5, 0, 0);

                txtTargetValue.Margin = new Thickness(0, 0, 0, 0);
                txtTargetValue2.Margin = new Thickness(0, 0, 0, 0);
            }
        }


    }
}
