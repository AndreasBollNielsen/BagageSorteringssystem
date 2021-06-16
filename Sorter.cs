using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    class Sorter
    {
        private Queue bagagesBuffer;


        public Queue BagagesBuffer
        {
            get { return bagagesBuffer; }
            set { bagagesBuffer = value; }
        }

        public Sorter()
        {

            bagagesBuffer = new Queue(10);
        }

        public void SortLuggage()
        {
            Random rand = new Random();
            while (true)
            {
                Monitor.Enter(Manager.GateBuffer);
                try
                {
                    if (Manager.GateBuffer.InternalLength > 0)
                    {
                        if (bagagesBuffer.InternalLength < 10)
                        {
                            Luggage luggage = Manager.GateBuffer.Remove();

                            int index = luggage.Flight.GetGate().IndexNumber;
                            Manager.gates[index].AddLuggage(luggage);

                            
                        }
                    }



                }
                finally
                {
                    Monitor.Exit(Manager.GateBuffer);
                }

                Thread.Sleep(rand.Next(1500, 3000));
            }


        }
    }
}
