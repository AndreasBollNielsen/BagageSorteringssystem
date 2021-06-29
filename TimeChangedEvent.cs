using System;
using System.Collections.Generic;
using System.Text;
namespace BagageSorteringssystem
{
    class TimeChangedEvent : EventArgs
    {
        //fields
        private DateTime currentTime;

        //properties
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        //constructor
        public TimeChangedEvent(DateTime currentTime)
        {
            this.currentTime = currentTime;
        }
    }
}
