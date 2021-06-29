using System;
using System.Collections.Generic;
using System.Text;
namespace BagageSorteringssystem
{
    class GateEventArgs: EventArgs
    {
        //fields
        private int numLuggage;
        private int maxluggage;
        private Gate.Status gateStatus;
        private int index;

        //properties
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public Gate.Status GateStatus
        {
            get { return gateStatus; }
            set { gateStatus = value; }
        }
        public int MaxLuggage
        {
            get { return maxluggage; }
            set { maxluggage = value; }
        }
        public int NumLuggage
        {
            get { return numLuggage; }
            set { numLuggage = value; }
        }

        //constructor
        public GateEventArgs(int numLuggage, int maxluggage, Gate.Status gateStatus,int index)
        {
            this.numLuggage = numLuggage;
            this.maxluggage = maxluggage;
            this.gateStatus = gateStatus;
            this.index = index;
        }
    }
}
