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
                        if (bagagesBuffer.InternalLength < bagagesBuffer.Length)
                        {
                            Luggage luggage = Manager.GateBuffer.Remove();
                            int index = luggage.Flight.GetGate(luggage.Flight);

                            //check if index is valid
                            if (index >= 0)
                            {
                                //check if gate is open
                                Monitor.Enter(Manager.gates[index]);
                                try
                                {
                                    if (Manager.gates[index].MyStatus == Gate.Status.open)
                                    {
                                        Manager.gates[index].AddLuggage(luggage);

                                    }
                                    else
                                    {
                                        Monitor.Enter(Manager.ReturnBuffer);
                                        try
                                        {
                                            Manager.ReturnBuffer.Add(luggage);
                                        }
                                        finally
                                        {
                                            Monitor.Enter(Manager.ReturnBuffer);
                                        }
                                    }
                                   // Console.WriteLine("index " + index + " destination " + luggage.Flight.Destination);
                                   
                                }
                                finally
                                {

                                    Monitor.Exit(Manager.gates[index]);
                                }
                            }

                            //add to return buffer if not valid
                            Monitor.Enter(Manager.ReturnBuffer);
                            try
                            {
                                Manager.ReturnBuffer.Add(luggage);
                            }
                            finally
                            {
                                Monitor.Enter(Manager.ReturnBuffer);
                            }



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
