using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class FlightPlanEventArgs: EventArgs
    {
        //fields
        private int index;
        private string gateName;
        private string flightStatus;

        //properties
        public string FlightStatus
        {
            get { return flightStatus; }
            set { flightStatus = value; }
        }
        public string GateName
        {
            get { return gateName; }
            set { gateName = value; }
        }
        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        //constructor
        public FlightPlanEventArgs(int index,string _gataName,string _status)
        {
            this.index = index;
            this.gateName = _gataName;
            this.flightStatus = _status;
        }
    }
}
