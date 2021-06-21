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

        //add luggage to return buffer
        void AddToReturnBuffer(Luggage luggage)
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

        public void SortLuggage()
        {
            Random rand = new Random();
            while (true)
            {
                //random choose which buffer to take from
                int chooser = rand.Next(2);
              //  chooser = 1;
                if(chooser >0)
                {
                    //sort from Check in buffers
                    Monitor.Enter(Manager.CheckInBuffer);
                    try
                    {
                        if (Manager.CheckInBuffer.InternalLength > 0)
                        {
                            if (bagagesBuffer.InternalLength < bagagesBuffer.Length)
                            {
                                Luggage luggage = Manager.CheckInBuffer.Remove();
                                int index = luggage.Flight.GetGate(luggage.Flight);

                                //check if index is valid
                                if (index >= 0)
                                {
                                    //check if gate is open
                                    Monitor.Enter(Manager.gates[index]);
                                    try
                                    {
                                        //Add luggage to gate buffer if open
                                        if (Manager.gates[index].MyStatus == Gate.Status.open && Manager.gates[index].NumLuggage < Manager.gates[index].Flight.MaxLuggage)
                                        {
                                            Monitor.Enter(Manager.GateBuffers[index]);
                                            try
                                            {
                                                Manager.GateBuffers[index].Add(luggage);
                                            }
                                            finally
                                            {
                                                Monitor.Exit(Manager.GateBuffers[index]);
                                            }
                                        }
                                        else
                                        {
                                            AddToReturnBuffer(luggage);
                                        }
                                    }
                                    finally
                                    {

                                        Monitor.Exit(Manager.gates[index]);
                                    }
                                }
                                else
                                {
                                    AddToReturnBuffer(luggage);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(Manager.CheckInBuffer);
                    }
                }
                //sort from return buffers
                else
                {
                    Monitor.Enter(Manager.ReturnBuffer);
                    try
                    {
                        if (Manager.ReturnBuffer.InternalLength > 0)
                        {
                            if (bagagesBuffer.InternalLength < bagagesBuffer.Length)
                            {
                                //get luggage from return buffer
                                Luggage luggage = Manager.ReturnBuffer.Remove();
                                int index = luggage.Flight.GetGate(luggage.Flight);

                                //check if index is valid
                                if (index >= 0)
                                {
                                    //check if gate is open
                                    Monitor.Enter(Manager.gates[index]);
                                    try
                                    {
                                        //Add luggage to gate buffer if open
                                        if (Manager.gates[index].MyStatus == Gate.Status.open && Manager.gates[index].NumLuggage < Manager.gates[index].Flight.MaxLuggage)
                                        {
                                            Monitor.Enter(Manager.GateBuffers[index]);
                                            try
                                            {
                                                Manager.GateBuffers[index].Add(luggage);
                                            }
                                            finally
                                            {
                                                Monitor.Exit(Manager.GateBuffers[index]);
                                            }
                                        }
                                        //add to return buffer if gate closed
                                        else
                                        {
                                            AddToReturnBuffer(luggage);
                                        }
                                    }
                                    finally
                                    {

                                        Monitor.Exit(Manager.gates[index]);
                                    }
                                }
                                //return the luggage if no gates available
                                else
                                {
                                    Manager.ReturnBuffer.Add(luggage);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(Manager.ReturnBuffer);
                    }
                }


                Thread.Sleep(rand.Next(100, 600));
            }
        }
    }
}
