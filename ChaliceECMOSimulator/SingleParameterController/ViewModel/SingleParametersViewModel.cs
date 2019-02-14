using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ChaliceECMOSimulator
{
    public class SingleParametersViewModel : INotifyPropertyChanged
    {
        // INotifyProperyChanged interface requirements
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public SingleParameterModel SingleParameter { get; } = new SingleParameterModel();

        // properties for the view
        private string _currentValue = "110";
        public string CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                OnPropertyChanged();
            }
        }

        private double _targetValue = 45;
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

        private string _controllerStatus = "idle";
        public string ControllerStatus
        {
            get { return _controllerStatus; }
            set { _controllerStatus = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _statusColor = new SolidColorBrush(Colors.DarkGreen);
        public  SolidColorBrush StatusColor
        {
            get { return _statusColor; }
            set { _statusColor = value; OnPropertyChanged(); }
        }

        private int ControllerState = 0;
        // commands for the view
        public ICommand MyCommand { private set; get; }

        // declare the update timer
        private readonly Windows.UI.Xaml.DispatcherTimer updateTimer = new Windows.UI.Xaml.DispatcherTimer();

        // viewModel constructor 
        public SingleParametersViewModel()
        {
            // declare the commands
            MyCommand = new RelayCommand(StartButtonPressed);

            // set the update timer to get the values from the model
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            updateTimer.Tick += UpdateTimer_Tick; ;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            ControllerState = SingleParameter.ControllerState;
            switch (ControllerState)
            {
                case 0: // idle
                    ControllerStatus = "idle";
                    StatusColor = new SolidColorBrush(Colors.DarkGreen);
                    break;
                case 1: // running
                    ControllerStatus = "running";
                    StatusColor = new SolidColorBrush(Colors.Red);
                    break;
                case 2: // waiting
                    ControllerStatus = "waiting";
                    StatusColor = new SolidColorBrush(Colors.Blue);
                    break;
            }
            CurrentValue = Math.Round(SingleParameter.CurrentValue,0).ToString();
        }

        public void StartButtonPressed()
        {
            SingleParameter.CalculateAutomation(TargetValue, 5, 10);
        }

    }

    class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }
        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// Determines whether this RelayCommand can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }
        /// <summary>
        /// Executes the RelayCommand on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, 
        /// this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            _execute();
        }
        /// <summary>
        /// Method used to raise the CanExecuteChanged event
        /// to indicate that the return value of the CanExecute
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }


}
