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
            if (data.Length > 0)
            {
                if (data[0].checkinBuf != null)
                {
                    for (int i = 0; i < data[0].checkinBuf.Length; i++)
                    {

                        Console.WriteLine($"{data[0].checkinBuf[i].PadRight(23)}{data[0].GateBuf[i].PadRight(25)}{data[0].ReturnBuf[i]}");

                    }
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine($"arrival buffer size: {data[0].ArrivalBufLength}");
            Console.WriteLine($"Checkin buffer size: {data[0].CheckInLength}");
            Console.WriteLine($"return buffer size: {data[0].ReturnBufLength}");
            //show gate & checkin info
            Console.WriteLine("{0}{1,20}{2,20}{3,20}{4,20}{5,20}", "Check-in name", "Check-in status", "Luggage Counter", "GateName", "Gate Status", "destination");
            for (int j = 0; j < data.Length; j++)
            {
                Console.WriteLine($"{data[j].checkInName.PadRight(18)}{data[j].checkInStatus.PadRight(20)}{data[j].LuggageCounter.PadRight(29)}{data[j].GateName.PadRight(15)}{data[j].GateStatus.PadRight(20)}{data[j].Destination.PadRight(20)}");
            }



        }

        ConsoleData[] CollectData()
        {
            //collect data from threads for debugging
            ConsoleData[] dataStream = new ConsoleData[Manager.gates.Length];
            int bufferlength = 50;
            //  bufferlength = GetlargestBufferSize();


            for (int n = 0; n < Manager.gates.Length; n++)
            {
                ConsoleData data = new ConsoleData();
                data.checkinBuf = new string[bufferlength];
                data.GateBuf = new string[bufferlength];
                data.ReturnBuf = new string[bufferlength];

                //initialize space in buffers
                for (int i = 0; i < data.checkinBuf.Length; i++)
                {
                    data.checkinBuf[i] = " ";
                    data.GateBuf[i] = " ";
                    data.ReturnBuf[i] = " ";
                }

                //check-In buffer
                Monitor.Enter(Manager.ArrivalBuffer);
                try
                {
                    if (Manager.ArrivalBuffer.InternalLength > 0)
                    {
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
                    data.CheckInLength = Manager.CheckInBuffer.InternalLength;
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

                }
                finally
                {

                    Monitor.Exit(Manager.gates[n]);
                }

                //from sorter buffer to return buffer
              /*  Monitor.Enter(Manager.ReturnBuffer);
                try
                {
                    if (Manager.ReturnBuffer.InternalLength > 0)
                    {
                        data.ReturnBufLength = Manager.ReturnBuffer.InternalLength;
                        for (int i = 0; i < Manager.ReturnBuffer.InternalLength; i++)
                        {
                            if (i < data.ReturnBuf.Length)
                            {
                                if (Manager.ReturnBuffer != null)
                                    data.ReturnBuf[i] = Manager.ReturnBuffer.Buffer[i].Flight.Destination.ToString();

                            }

                        }

                    }
                }
                finally
                {
                    Monitor.Exit(Manager.ReturnBuffer);
                }*/
               
                dataStream[n] = data;
            }

            return dataStream;
        }

        int GetlargestBufferSize()
        {
            int[] bufferslength = new int[3];

            int returnlength = 0;
            //arrival buffer
            Monitor.Enter(Manager.ArrivalBuffer);
            try
            {
                bufferslength[0] = Manager.ArrivalBuffer.InternalLength;
            }
            finally
            {

                Monitor.Exit(Manager.ArrivalBuffer);
            }

            //return buffer
            Monitor.Enter(Manager.ReturnBuffer);
            try
            {
                bufferslength[1] = Manager.ReturnBuffer.InternalLength;

            }
            finally
            {
                Monitor.Exit(Manager.ReturnBuffer);

            }

            //checkin buffer
            Monitor.Enter(Manager.CheckInBuffer);
            try
            {
                bufferslength[2] = Manager.CheckInBuffer.InternalLength;

            }
            finally
            {
                Monitor.Exit(Manager.CheckInBuffer);

            }




            //get the largest number in array
            for (int i = 0; i < bufferslength.Length - 1; i++)
            {
                if (bufferslength[i] > bufferslength[i + 1])
                {
                    returnlength = bufferslength[i];
                }
            }

            return returnlength;
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
        public int ReturnBufLength;

        public int CheckInLength;
    }
}
