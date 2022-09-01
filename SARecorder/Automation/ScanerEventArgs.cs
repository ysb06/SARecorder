using System;
using System.Collections.Generic;
using System.Text;

namespace SARecorder.Automation
{
    public class ScanerEventArgs : EventArgs
    {
        public bool SAGATState { get; private set; }
        public double[] Extras { get; private set; }

        public ScanerEventArgs(bool SAGAT, double[] extras)
        {
            SAGATState = SAGAT;
            Extras = extras;
        }
    }
}
