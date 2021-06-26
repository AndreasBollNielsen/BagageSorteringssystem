using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class FlightPlanEventArgs: EventArgs
    {
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }



        public FlightPlanEventArgs(int index)
        {
            this.index = index;

        }
    }
}
