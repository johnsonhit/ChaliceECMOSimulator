using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChaliceECMOSimulator
{
    public class SingleParametersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public SingleParameterModel DefaultSingleParameter { get; } = new SingleParameterModel();

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


    }
}
