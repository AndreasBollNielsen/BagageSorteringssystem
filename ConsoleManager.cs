using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class ConsoleManager
    {
        public void PrintData(string[] checkin, string[] gateBuf, string[] gate1, string[] gate2)
        {
            Console.WriteLine("{0}{1,20}{2,20}{3,20}{4,20}", "CheckIn Buffer", "Gate Buffer", "GateName", "Luggage Counter", "Status");

          

           // Console.WriteLine($"{gate1[0].PadRight(80)}");

            for (int i = 0; i < 10; i++)
            {
                // Console.WriteLine("{0} {1} {2,20} {3,20}", checkin[i], gateBuf[i], gates[0], gates[1]);
                if (checkin[i] != null)
                {
                    // padding = padding + gateBuf[i].Length;

                    if (checkin[i] == null)
                    {
                        checkin[i] = " ";
                    }

                    if (gateBuf[i] == null)
                    {
                        gateBuf[i] = " ";
                    }

                    if (gate1[0] == null)
                    {
                        gate1[0] = " ";
                    }

                    Console.WriteLine($"{checkin[i].PadRight(20)} {gateBuf[i].PadRight(25)} {gate1[0].PadRight(12)}{gate1[1].PadRight(30)}{gate1[2].PadRight(12)}");
                    Console.WriteLine($"{checkin[i].PadRight(20)} {gateBuf[i].PadRight(25)} {gate2[0].PadRight(12)}{gate2[1].PadRight(30)}{gate2[2].PadRight(12)}");
                }



            }


        }
    }
}
