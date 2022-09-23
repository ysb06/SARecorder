using System;
using System.Collections.Generic;
using System.Text;

namespace SARecorder.Automation
{
    public class ScanerEventArgs : EventArgs
    {
        public int CurrentStep { get; private set; }
        public bool SAGATState { get; private set; }
        public bool AutonomousState { get; private set; }
        public int CurrentScenario { get; private set; }
        public double[] Extras { get; private set; }

        public ScanerEventArgs(bool SAGAT, bool Auto, int step, int scenario, double[] extras)
        {
            SAGATState = SAGAT;
            AutonomousState = Auto;
            Extras = extras;
            CurrentStep = step;
        }
    }
}
