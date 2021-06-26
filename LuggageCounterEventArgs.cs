using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class LuggageCounterEventArgs: EventArgs
    {
        private int luggageCount;

        public int LuggageCount
        {
            get { return luggageCount; }
            set { luggageCount = value; }
        }

        public LuggageCounterEventArgs(int luggageCount)
        {
            this.luggageCount = luggageCount;
        }
    }
}
