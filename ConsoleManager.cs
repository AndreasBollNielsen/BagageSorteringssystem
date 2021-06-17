using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class ConsoleManager
    {
        public void PrintData(ConsoleData[] data)
        {
            //show buffer info
            Console.WriteLine("{0}{1,20}{2,20}", "CheckIn Buffer", "Gate Buffer","Return Buffer");
            for (int i = 0; i < data[0].checkinBuf.Length; i++)
            {
                if (data[0].checkinBuf != null)
                {
                    Console.WriteLine($"{data[0].checkinBuf[i].PadRight(23)}{data[0].GateBuf[i].PadRight(25)}{data[0].ReturnBuf[i]}");
                }
            }

            //show gate & checkin info
            Console.WriteLine("{0}{1,20}{2,20}{3,20}{4,20}", "GateName", "Check-in name", "Check-in status", "Luggage Counter", "Status");
            for (int j = 0; j < data.Length; j++)
            {
                Console.WriteLine($"{data[j].GateName.PadRight(15)}{data[j].checkInName.PadRight(18)}{data[j].checkInStatus.PadRight(20)}{data[j].LuggageCounter.PadRight(29)}{data[j].GateStatus.PadRight(20)}");
            }
        }
    }

    //data storage for debugging
    class ConsoleData
    {
        public string[] checkinBuf;
        public string[] GateBuf;
        public string[] ReturnBuf;
        public string GateName;
        public string checkInName;
        public string checkInStatus;
        public string LuggageCounter;
        public string GateStatus;
    }
}
