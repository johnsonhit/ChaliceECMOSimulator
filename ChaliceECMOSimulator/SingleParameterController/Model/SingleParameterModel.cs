using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChaliceECMOSimulator
{
    public class SingleParameterModel
    {
        public double CurrentValue { get; set; } = 150;
        public double TargetValue { get; set; } = 150;
        public double UpperLimit { get; set; } = 300;
        public double LowerLimit { get; set; } = 0;
        public int ControllerState { get; set; } = 0;       // 0 = idle, 1 = running, 2 = waiting

        double deltaValue = 0;
        double timeIn = 0;
        double timeAt = 0;

        private readonly Windows.UI.Xaml.DispatcherTimer updateTimer = new Windows.UI.Xaml.DispatcherTimer();

        public SingleParameterModel()
        {

            updateTimer.Interval = new TimeSpan(0, 0, 0, 1);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

          
        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            UpdateParameters();
        }

        private void UpdateParameters()
        {
            if (timeAt <= 0)
            {
                if (deltaValue != 0)
                {
                    timeAt = 0;
                    ControllerState = 1;
                    CurrentValue += deltaValue;
                    if (Math.Abs(CurrentValue - TargetValue) < Math.Abs(deltaValue))
                    {
                        ControllerState = 0;
                        timeIn = 0;
                        deltaValue = 0;
                        CurrentValue = TargetValue;
                    }
                }
            } else
            {
                ControllerState = 2;
                timeAt--;
            }

        }
        public void CalculateAutomation(double tv, double tAt, double tIn)
        {
            TargetValue = tv;
            timeIn = tIn;
            timeAt = tAt;

            if (tAt > 0) timeAt = tAt;
            if (tIn > 0)
            {
                deltaValue = (TargetValue - CurrentValue) / tIn;
            } else
            {
                deltaValue = (TargetValue - CurrentValue);
            }
           
        }
    }

   
}
