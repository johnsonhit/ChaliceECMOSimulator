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
    public sealed partial class ParameterController : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        bool _test1 = false;
        public bool Test1 { get { return _test1; } set { _test1 = value; OnPropertyChanged(); } }

        bool _test2 = false;
        public bool Test2 { get { return _test2; } set { _test2 = value; OnPropertyChanged(); } }

        public double CurrentValue { get; set; } = 100;
        public double CurrentValue2 { get; set; } = 100;
        public int ControllerState { get; set; } = 0;       // 0 = idle, 1 = running, 2 = waiting, 3 = armed

        public double Incrementor { get; set; } = 1;
        double deltaValue = 0;
        double deltaValue2 = 0;

        bool _showLimits = false;
        public bool ShowLimits
        {
            get
            {
                return _showLimits;
            }
            set
            {
                _showLimits = value;
                OnPropertyChanged();
            }
        }

        bool _showAlarmLimits = false;
        public bool ShowAlarmLimits
        {
            get
            {
                return _showAlarmLimits;
            }
            set
            {
                _showAlarmLimits = value;
                OnPropertyChanged();
            }
        }

        bool sliderLock = false;

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

        bool _fluctStatus1 = false;
        public bool FluctStatus1 { get { return _fluctStatus1; }
            set {
                _fluctStatus1 = value;
                if (_fluctStatus1)
                {
                    FluctStatus = true;
                    FluctStatusString1 = "on";
                    baseValue1 = CurrentValue;
                } else
                {
                    FluctStatusString1 = "off";
                }

                if (_fluctStatus1 == false && FluctStatus2 == false) FluctStatus = false;
                OnPropertyChanged();
            }
        }

        bool _fluctStatus2 = false;
        public bool FluctStatus2 { get { return _fluctStatus2; }
            set {
                _fluctStatus2 = value;
                if (_fluctStatus2)
                {
                    FluctStatus = true;
                    FluctStatusString2 = "on";
                    baseValue2 = CurrentValue2;
                }
                else
                {
                    FluctStatusString2 = "off";
                }
                if (FluctStatus1 == false && FluctStatus2 == false) FluctStatus = false;
                OnPropertyChanged();
            }
        }

        string _fluctStatusString1 = "off";
        public string FluctStatusString1 { get { return _fluctStatusString1; } set { _fluctStatusString1 = value; OnPropertyChanged(); } }

        string _fluctStatusString2 = "off";
        public string FluctStatusString2 { get { return _fluctStatusString2; } set { _fluctStatusString2 = value; OnPropertyChanged(); } }

        bool _fluctMode = false;
        public bool FluctMode { get { return _fluctMode; }
            set {
                _fluctMode = value;
                if (value)
                {
                    FluctModeString = "sinus";
                } else
                {
                    FluctModeString = "random";
                }
                OnPropertyChanged(); }
        }

        string _fluctModeString = "random";
        public string FluctModeString {
            get { return _fluctModeString; }
            set { 
                _fluctModeString = value;
 
                OnPropertyChanged(); }
        }

        double fluctWidth = 1;
        double fluctTime = 500;
        double fluctTimeCounter = 0;
        double baseValue1 = 0;
        double baseValue2 = 0;

        double fluctAngle = 0;

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

        private string _title = "TITLE";
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

        private string _unit = "unit";
        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        private double _targetValue = 100;
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

        private double _targetValue2 = 100;
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

        private double _upperLimit = 100;
        public double UpperLimit
        {
            get { return _upperLimit; }
            set
            {
                _upperLimit = value;
                OnPropertyChanged();
            }
        }

        private double _upperAlarmLimit = 100;
        public double UpperAlarmLimit
        {
            get { return _upperAlarmLimit; }
            set
            {
                _upperAlarmLimit = value;
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

        private double _lowerAlarmLimit = 0;
        public double LowerAlarmLimit
        {
            get { return _lowerAlarmLimit; }
            set
            {
                _lowerAlarmLimit = value;
                OnPropertyChanged();
            }
        }

        bool _alarmsEnabled = false;
        public bool AlarmsEnabled { get { return _alarmsEnabled; } set { _alarmsEnabled = value; OnPropertyChanged(); } }

        private string _controllerStatus = "START";
        public string ControllerStatus
        {
            get { return _controllerStatus; }
            set { _controllerStatus = value; OnPropertyChanged(); }
        }

        int selectedSlider = 0;
        private SolidColorBrush _headerColor = new SolidColorBrush(Colors.DarkSlateGray);
        public SolidColorBrush HeaderColor { get { return _headerColor; } set { _headerColor = value; OnPropertyChanged(); } }

        private SolidColorBrush _selColor1 = new SolidColorBrush(Colors.White);
        public SolidColorBrush SelColor1 { get { return _selColor1; } set { _selColor1 = value; OnPropertyChanged(); } }

        private SolidColorBrush _selColor2 = new SolidColorBrush(Colors.DarkGray);
        public SolidColorBrush SelColor2 { get { return _selColor2; } set { _selColor2 = value; OnPropertyChanged(); } }

        private SolidColorBrush _statusColor = new SolidColorBrush(Colors.DarkGray);
        public SolidColorBrush StatusColor { get { return _statusColor; } set { _statusColor = value; OnPropertyChanged(); } }
        private SolidColorBrush _statusColorText = new SolidColorBrush(Colors.White);
        public SolidColorBrush StatusColorText { get { return _statusColorText; } set { _statusColorText = value; OnPropertyChanged(); } }

        private readonly Windows.UI.Xaml.DispatcherTimer updateTimer = new Windows.UI.Xaml.DispatcherTimer();
        private readonly Windows.UI.Xaml.DispatcherTimer fluctTimer = new Windows.UI.Xaml.DispatcherTimer();

        public ParameterController()
        {
            this.InitializeComponent();

            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            fluctTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            fluctTimer.Tick += FluctTimer_Tick;
 

            initialized = true;

            ControllerMode(DualControllerMode);

        }

        private void FluctTimer_Tick(object sender, object e)
        {
            // fluctMode false = random and true = sinus

            Random seed = new Random();

            if (FluctStatus)
            {
                double delta1 = 0;
                double delta2 = 0;

                if (FluctMode)
                {
                    // sinus fluct mode
                    double sineStepsize = (2 * Math.PI) / fluctTime * 50;
                    fluctAngle += sineStepsize;
                    if (fluctAngle > 2 * Math.PI) fluctAngle = 0;
                    delta1 = Math.Sin(fluctAngle) * fluctWidth;
                    if (FluctStatus1)
                    {
                        if (baseValue1 + delta1 <= UpperLimit && baseValue2 + delta1 >= LowerLimit)
                        {
                            CurrentValue = baseValue1 + delta1;
                        }
                    }
                    if (FluctStatus2)
                    {
                        if (baseValue2 + delta1 <= UpperLimit && baseValue2 + delta1 >= LowerLimit)
                        {
                            CurrentValue2 = baseValue2 + delta1;
                        }
                    }
                } else
                {
                    if (fluctTimeCounter > fluctTime)
                    {
                        fluctTimeCounter = 0;
                        int sign = seed.Next(2);

                        switch (sign)
                        {
                            case 0:
                                delta1 = seed.NextDouble() * (baseValue1 / 100) * fluctWidth;
                                delta2 = seed.NextDouble() * (baseValue2 / 100) * fluctWidth;
                                break;
                            case 1:
                                delta1 = seed.NextDouble() * -(baseValue1/ 100) * fluctWidth;
                                delta2 = seed.NextDouble() * -(baseValue2 /100) * fluctWidth;
                                break;
                        }

                        if (FluctStatus1)
                        {
                            if (CurrentValue + delta1 <= UpperLimit && CurrentValue + delta1 >= LowerLimit)
                            {
                                CurrentValue = CurrentValue + delta1;
                            }
                        }
                        if (FluctStatus2)
                        {
                            if (CurrentValue2 + delta1 <= UpperLimit && CurrentValue2 + delta1 >= LowerLimit)
                            {
                                CurrentValue2 = CurrentValue2 + delta1;
                            }
                        }
                    } else
                    {
                        fluctTimeCounter += 50;
                    }

                }


            }

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

   
                if (DualControllerMode)
                {
              
                    CurrentValueString = ((int)CurrentValue).ToString() + "/" + ((int)CurrentValue2).ToString();
                } else
                {
                    CurrentValueString = ((int)CurrentValue).ToString();
                }


            UpdateParameters();

            CheckAlarms();

        }

        private void CheckAlarms()
        {
            if (AlarmsEnabled)
            {
                bool soundAlarm = false;

                if (CurrentValue > UpperAlarmLimit || CurrentValue < LowerAlarmLimit)
                {
                    soundAlarm = true;
                }

                if (CurrentValue2 > UpperAlarmLimit || CurrentValue2 < LowerAlarmLimit)
                {
                    if (DualControllerMode)
                    {
                        soundAlarm = true;
                    }
                }

                if (soundAlarm && blinker)
                {
                    HeaderColor = new SolidColorBrush(Colors.DarkRed);
                } else
                {
                    HeaderColor = new SolidColorBrush(Colors.DarkSlateGray);
                }
            } else
            {
                HeaderColor = new SolidColorBrush(Colors.DarkSlateGray);
            }
        }
        private void UpdateParameters()
        {
            bool reachedDestination1 = true;
            bool reachedDestination2 = true;

            if (running)
            {
                if (timeAt <= 0)
                {
                    timeAt = 0;
                    if (blinker) timeIn--;

                    // if the deltavalue is not zero that means we have not reached the destination yet
                    if (deltaValue != 0)
                    {   
                        // check whether we're there yet or within deltavalue reange
                        if (Math.Abs(CurrentValue - TargetValue) < Math.Abs(deltaValue))
                        {
                            // we're there so the deltavalue can be set to zero
                            deltaValue = 0;
                            // set the current value to the exact targetvalue
                            CurrentValue = TargetValue;
                            baseValue1 = CurrentValue;
                            // flag that parameter 1 has reached it's destination
                            reachedDestination1 = true;
                        } else
                        {
                            // if did not reach the destination van parameter 1 yet then increase the currentvalue with the delta value 
                            CurrentValue += deltaValue;
                            baseValue1 = CurrentValue;
                            // flag that we didn't reach our destination yet
                            reachedDestination1 = false;
                        }
                    } else
                    {
                        // if the deltavalue = 0 this means we have reach our destination for parameter 1
                        reachedDestination1 = true;
                    }

                    if (DualControllerMode)
                    {
                        reachedDestination2 = false;
                        // if the deltavalue 2 is not zero that means we have not reached the destination yet
                        if (deltaValue2 != 0)
                        {
                            // check whether we're there yet or within deltavalue reange
                            if (Math.Abs(CurrentValue2 - TargetValue2) < Math.Abs(deltaValue2))
                            {
                                // we're there so the deltavalue can be set to zero
                                deltaValue2 = 0;
                                // set the current value to the exact targetvalue
                                CurrentValue2 = TargetValue2;
                                baseValue2 = CurrentValue2;
                                // flag that parameter 2 has reached it's destination
                                reachedDestination2 = true;
                            }
                            else
                            {
                                // if did not reach the destination van parameter 1 yet then increase the currentvalue with the delta value 
                                CurrentValue2 += deltaValue2;
                                baseValue2 = CurrentValue2;
                                // flag that we didn't reach our destination yet
                                reachedDestination2 = false;
                            }
                        }
                        else
                        {
                            // if the deltavalue = 0 this means we have reach our destination for parameter 1
                            reachedDestination2 = true;
                        }
                    }
                
                    // if reached the reached destination flags are both true than reset the control to direct control
                    if (reachedDestination1 && reachedDestination2)
                    {
                  
                        ResetToDirectControl();

                    } else
                    {
                        // set the controller state to running
                        ControllerState = 1;
                    }
                } else
                {
                    // if time to start automation has not elapsed yet flag the controller state to waiting and decrease the time at counter
                    ControllerState = 2;
                    if (blinker) timeAt--;
                }
            }
        }

        public void CalculateAutomation()
        {
            bool deltaIsZero1 = true;
            bool deltaIsZero2 = true;

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

                deltaIsZero1 = false;
            } 

            if (DualControllerMode)
            {
                if (Math.Abs(TargetValue2 - CurrentValue2) > 0)
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
                        deltaValue2 = ((TargetValue2 - CurrentValue2) / timeIn) / 2;
                    }
                    else
                    {
                        deltaValue2 = (TargetValue2 - CurrentValue2);
                    }

                    deltaIsZero2 = false;
                }
            }
      
            if (deltaIsZero1 && deltaIsZero2)
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
            Slider1Tapped();
        }
        void Slider1Tapped()
        {
            if (initialized)
            {
                selectedSlider = 0;
                SelColor1 = new SolidColorBrush(Colors.White);
                SelColor2 = new SolidColorBrush(Colors.DarkGray);
                ResetAutomation();
            }
        }
        private void SliTargetValue2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Slider2Tapped();
        }
        void Slider2Tapped()
        {
            if (initialized)
            {
                selectedSlider = 1;
                SelColor1 = new SolidColorBrush(Colors.DarkGray);
                SelColor2 = new SolidColorBrush(Colors.White);
                ResetAutomation();
            }
        }
        void ResetToDirectControl()
        {
            running = false;
            ControllerState = 0;
            timeIn = 0;
            sliTargetValue.Value = Math.Round(CurrentValue,0);
            sliTargetValue2.Value = Math.Round(CurrentValue2, 0);
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
                    baseValue1 = CurrentValue;
                    CurrentValue2 = TargetValue2;
                    baseValue2 = CurrentValue2;
                }

                ControllerState = 0;

                if (TargetValue != CurrentValue)
                {
                    ControllerState = 3;
                }
                if (TargetValue2 != CurrentValue2)
                {
                    ControllerState = 3;
                }


            }

            running = false;
        }

        private void ButPlus_Click(object sender, RoutedEventArgs e)
        {
            if (DualControllerMode)
            {
                switch (selectedSlider)
                {
                    case 0:
                        if (TargetValue + Incrementor <= UpperLimit) TargetValue += Incrementor;
                        break;
                    case 1:
                        if (TargetValue2 + Incrementor <= UpperLimit) TargetValue2 += Incrementor;
                        break;
                    case 3:
                        if (TargetValue + Incrementor <= UpperLimit) TargetValue += Incrementor;
                        if (TargetValue2 + Incrementor <= UpperLimit) TargetValue2 += Incrementor;
                        break;

                }
               
           
            } else
            {
                if (TargetValue + Incrementor <= UpperLimit) TargetValue += Incrementor;
            }
           
        }

        private void ButMinus_Click(object sender, RoutedEventArgs e)
        {
            if (DualControllerMode)
            {
                switch (selectedSlider)
                {
                    case 0:
                        if (TargetValue - Incrementor >= LowerLimit) TargetValue -= Incrementor;
                        break;
                    case 1:
                        if (TargetValue2 - Incrementor >= LowerLimit) TargetValue2 -= Incrementor;
                        break;
                    case 3:
                        if (TargetValue - Incrementor >= LowerLimit) TargetValue -= Incrementor;
                        if (TargetValue2 - Incrementor >= LowerLimit) TargetValue2 -= Incrementor;
                        break;
                }
            } else
            {
                if (TargetValue - Incrementor >= LowerLimit) TargetValue -= Incrementor;
            }
            
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
            FluctStatus1 = !FluctStatus1;
            if (FluctStatus1)
            {
                FluctStatusString1 = "on";
                fluctTimer.Start();
                bool test1 = (double.TryParse(txtFluctWidth.Text, out fluctWidth));
                bool test2 = (double.TryParse(txtFluctTime.Text, out fluctTime));

                if (test1 == false || test2 == false)
                {
                    FluctStatus1 = false;
                    FluctStatusString1 = "fail";
                }

            } else
            {
                FluctStatusString1 = "off";
            }

            if (FluctStatus1 || FluctStatus2)
            {
                FluctStatus = true;
                fluctTimer.Start();
            }

            if (FluctStatus1 == false && FluctStatus2 == false)
            {
                FluctStatus = false;
                fluctTimer.Stop();
            }
        }

        private void ButFluct2_Click(object sender, RoutedEventArgs e)
        {
            FluctStatus2 = !FluctStatus2;
            if (FluctStatus2)
            {
                FluctStatusString2 = "on";
            
                bool test1 = (double.TryParse(txtFluctWidth.Text, out fluctWidth));
                bool test2 = (double.TryParse(txtFluctTime.Text, out fluctTime));

                if (test1 == false || test2 == false)
                {
                    FluctStatus2 = false;
                    FluctStatusString2 = "fail";
                }

            }
            else
            {
                FluctStatusString2 = "off";
            }

            if (FluctStatus1 || FluctStatus2)
            {
                FluctStatus = true;
                fluctTimer.Start();
            }

            if (FluctStatus1 == false && FluctStatus2 == false)
            {
                FluctStatus = false;
                fluctTimer.Stop();
            }

        }


        private void TxtFluctWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            FluctStatus1 = false;
            FluctStatus2 = false;

        }

        private void TxtFluctTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            FluctStatus1 = false;
            FluctStatus2 = false;

        }

        private void ButFluctMode_Click(object sender, RoutedEventArgs e)
        {
            FluctMode = !FluctMode;
            if (FluctMode)
            {
                // sinus
                txtFlucDeltaTitle.Text = " > width";
                txtFlucTimeTitle.Text = " > cycle (ms)";
            } else
            {
                // random
                txtFlucDeltaTitle.Text = " > width (%)";
                txtFlucTimeTitle.Text = " > freq. (ms)";
            }
        }

        private void ButAuto_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ButModel_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ControllerMode(bool dualonoff)
        {
            DualControllerMode = dualonoff;

            if (DualControllerMode)
            {
                // dual controller mode
                FluctStatus2 = false;
                FluctStatus1 = false;

                butFluct.Width = 25;
                butFluct2.Visibility = Visibility.Visible;

                butLock.Visibility = Visibility.Visible;
                sliTargetValue2.Visibility = Visibility.Visible;
                txtTargetValue2.Visibility = Visibility.Visible;
                sliTargetValue.Margin = new Thickness(-30, 5, 0, 0);
                sliTargetValue2.Margin = new Thickness(30, 5, 0, 0);

                txtTargetValue.Margin = new Thickness(-30, 0, 0, 0);
                txtTargetValue2.Margin = new Thickness(30, 0, 0, 0);
            } else
            {
                FluctStatus2 = false;
                FluctStatus1 = false;
                butFluct2.Visibility = Visibility.Collapsed;
                butFluct.Width = 50;
                butLock.Visibility = Visibility.Collapsed;
                sliTargetValue2.Visibility = Visibility.Collapsed;
                txtTargetValue2.Visibility = Visibility.Collapsed;

                sliTargetValue.Margin = new Thickness(0, 5, 0, 0);
                sliTargetValue2.Margin = new Thickness(0, 5, 0, 0);

                txtTargetValue.Margin = new Thickness(0, 0, 0, 0);
                txtTargetValue2.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void ButLock_Click(object sender, RoutedEventArgs e)
        {
            sliderLock = !sliderLock;

            if (sliderLock)
            {
                selectedSlider = 3;
                SelColor1 = new SolidColorBrush(Colors.White);
                SelColor2 = new SolidColorBrush(Colors.White);
            } else
            {
                selectedSlider = 0;
                SelColor1 = new SolidColorBrush(Colors.White);
                SelColor2 = new SolidColorBrush(Colors.DarkGray);
            }
           
        }

        private void ButUpperLower_Click(object sender, RoutedEventArgs e)
        {
            if (initialized)
                ShowParameterLimits();

        }
        void ShowParameterLimits()
        {
            if (ShowAlarmLimits) ShowParameterAlarmLimits(); 
            ShowLimits = !ShowLimits;

            if (ShowLimits)
            {
                txtUpperLimit.Visibility = Visibility.Visible;
                txtLowerLimit.Visibility = Visibility.Visible;
            }
            else
            {
                bool result1 = double.TryParse(txtUpperLimit.Text, out double ul);
                bool result2 = double.TryParse(txtLowerLimit.Text, out double ll);
                if (result1 && result2)
                {
                    UpperLimit = ul;
                    LowerLimit = ll;
    
                } else
                {
                    txtUpperLimit.Text = UpperLimit.ToString();
                    txtLowerLimit.Text = LowerLimit.ToString();

                }
                txtUpperLimit.Visibility = Visibility.Collapsed;
                txtLowerLimit.Visibility = Visibility.Collapsed;
            }
        }

        private void ButAlarms_Click(object sender, RoutedEventArgs e)
        {
           if (initialized)
            ShowParameterAlarmLimits();
        }

        void ShowParameterAlarmLimits()
        {
            if (ShowLimits) ShowParameterLimits();
            ShowAlarmLimits = !ShowAlarmLimits;

            if (ShowAlarmLimits)
            {
                txtUpperAlarmLimit.Visibility = Visibility.Visible;
                txtLowerAlarmLimit.Visibility = Visibility.Visible;
            }
            else
            {
                // check wheter the inputs are ok
                bool result1 = double.TryParse(txtUpperAlarmLimit.Text, out double ul);
                bool result2 = double.TryParse(txtLowerAlarmLimit.Text, out double ll);
                if (result1 && result2)
                {
                    UpperAlarmLimit = ul;
                    LowerAlarmLimit = ll;

                }
                else
                {
                    txtUpperAlarmLimit.Text = UpperAlarmLimit.ToString();
                    txtLowerAlarmLimit.Text = LowerAlarmLimit.ToString();

                }


                txtUpperAlarmLimit.Visibility = Visibility.Collapsed;
                txtLowerAlarmLimit.Visibility = Visibility.Collapsed;
            }
        }

    
    }
}
