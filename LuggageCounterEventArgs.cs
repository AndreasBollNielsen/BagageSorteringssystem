using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class LuggageCounterEventArgs: EventArgs
    {
        //fields
        private int luggageCount;

        //properties
        public int LuggageCount
        {
            get { return luggageCount; }
            set { luggageCount = value; }
        }

        //constructor
        public LuggageCounterEventArgs(int luggageCount)
        {
            this.luggageCount = luggageCount;
        }
    }
}
