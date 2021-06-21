using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class ConsoleManager
    {
        public void PrintData()
        {
            ConsoleData[] data = CollectData();
           
            //show buffer info
            Console.WriteLine("{0}{1,20}{2,20}", "CheckIn Buffer", "Gate Buffer", "Return Buffer");
            if(data.Length > 0)
            {
                if (data[0].checkinBuf != null)
                {
                    for (int i = 0; i < data[0].checkinBuf.Length; i++)
                    {
                        if (data[0].checkinBuf != null)
                        {
                            Console.WriteLine($"{data[0].checkinBuf[i].PadRight(23)}{data[0].GateBuf[i].PadRight(25)}{data[0].ReturnBuf[i]}");
                        }
                    }
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine($"buffer size: {data[0].ArrivalBufLength}");
            //show gate & checkin info
            Console.WriteLine("{0}{1,20}{2,20}{3,20}{4,20}{5,20}", "Check-in name", "Check-in status", "Luggage Counter", "GateName", "Gate Status","destination");
            for (int j = 0; j < data.Length; j++)
            {
                Console.WriteLine($"{data[j].checkInName.PadRight(18)}{data[j].checkInStatus.PadRight(20)}{data[j].LuggageCounter.PadRight(29)}{data[j].GateName.PadRight(15)}{data[j].GateStatus.PadRight(20)}{data[j].Destination.PadRight(20)}");
            }
        }

        ConsoleData[] CollectData()
        {
            //collect data from threads for debugging
            ConsoleData[] dataStream = new ConsoleData[Manager.gates.Length];
            for (int n = 0; n < Manager.gates.Length; n++)
            {
                ConsoleData data = new ConsoleData();

                //check-In buffer
                Monitor.Enter(Manager.ArrivalBuffer);
                try
                {
                    if (Manager.ArrivalBuffer.InternalLength > 0)
                    {
                        data.checkinBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                        for (int i = 0; i < Manager.ArrivalBuffer.InternalLength; i++)
                        {
                            if (i < data.checkinBuf.Length)
                            {
                                data.checkinBuf[i] = Manager.ArrivalBuffer.Buffer[i].Flight.Destination.ToString();

                            }

                        }
                        data.ArrivalBufLength = Manager.ArrivalBuffer.InternalLength;
                    }
                }
                finally
                {
                    Monitor.Exit(Manager.ArrivalBuffer);

                }

                //from check-In counter
                Monitor.Enter(Manager.checkins[n]);
                try
                {
                    data.checkInName = Manager.checkins[n].Name;
                    data.checkInStatus = Manager.checkins[n].MyStatus.ToString();
                }
                finally
                {
                    Monitor.Exit(Manager.checkins[n]);
                }

                //from check-In to gate buffer
                Monitor.Enter(Manager.CheckInBuffer);
                try
                {
                    data.GateBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                    for (int i = 0; i < Manager.CheckInBuffer.InternalLength; i++)
                    {
                        if (i < data.GateBuf.Length)
                        {
                            data.GateBuf[i] = Manager.CheckInBuffer.Buffer[i].Flight.Destination.ToString();

                        }
                    }
                }
                finally
                {

                    Monitor.Exit(Manager.CheckInBuffer);
                }


                //from gate buffer to flight
                Monitor.Enter(Manager.gates[n]);
                try
                {
                    data.GateName = Manager.gates[n].GateName;
                    data.LuggageCounter = Manager.gates[n].NumLuggage.ToString() + "/" + Manager.gates[n].Flight.MaxLuggage;
                    data.GateStatus = Manager.gates[n].MyStatus.ToString();
                    data.Destination = Manager.gates[n].Flight.Destination;
                    data.ReturnBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };

                }
                finally
                {

                    Monitor.Exit(Manager.gates[n]);
                }

                //from sorter buffer to return buffer
                if (Manager.ReturnBuffer.InternalLength > 0)
                {
                    for (int i = 0; i < Manager.ReturnBuffer.InternalLength; i++)
                    {
                        if (i < data.ReturnBuf.Length)
                        {
                            data.ReturnBuf[i] = Manager.ReturnBuffer.Buffer[i].Flight.Destination.ToString();

                        }

                    }

                }

                dataStream[n] = data;
            }

            return dataStream;
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
        public string Destination;
        public int ArrivalBufLength;
    }
}
