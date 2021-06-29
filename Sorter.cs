using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    class Sorter
    {
        //fields
        private Queue bagagesBuffer;

        //properties
        public Queue BagagesBuffer
        {
            get { return bagagesBuffer; }
            set { bagagesBuffer = value; }
        }

        //constructor
        public Sorter()
        {

            bagagesBuffer = new Queue(10);
        }

        //sort luggage from buffer
        public void SortLuggage()
        {
            Random rand = new Random();
            while (true)
            {
                //random choose which buffer to take from
                int chooser = rand.Next(2);

                // if returnbuffer is empty don´t use it
                if (!checkReturnbuffer())
                {
                    chooser = 1;
                }

                //sort luggage from checkin buffer
                if (chooser > 0)
                {
                    SortByGate();
                }
                //sort from return buffer
                else
                {
                    SortByReturn();
                }

                
                Thread.Sleep(rand.Next(10, 60));
            }
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


        //check if returnbuffer contains content
        bool checkReturnbuffer()
        {
            Monitor.Enter(Manager.ReturnBuffer);
            try
            {
                if (Manager.ReturnBuffer.InternalLength > 0)
                {
                    return true;
                }
            }
            finally
            {
                Monitor.Exit(Manager.ReturnBuffer);
            }

            return false;
        }

        //sort luggage to an open gate or return buffer
        void SortByGate()
        {
            //sort from Check in buffers
            Monitor.Enter(Manager.CheckInBuffer);
            try
            {
                if (Manager.CheckInBuffer.InternalLength > 0)
                {
                    //get the luggage from checkin buffer
                    Luggage luggage = Manager.CheckInBuffer.Remove();
                    int index = luggage.Flight.GetGate(luggage.Flight, true);

                    //if index is above -1 the gate is open
                    if (index >= 0)
                    {
                        //check if gate is open
                        Monitor.Enter(Manager.gates[index]);
                        try
                        {
                            //Add luggage to gate buffer
                            if (Manager.gates[index].NumLuggage < Manager.gates[index].Flight.MaxLuggage)
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
                        }
                        finally
                        {
                            Monitor.Exit(Manager.gates[index]);
                        }
                    }
                    else
                    {
                       //add luggage to return buffer
                        AddToReturnBuffer(luggage);
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.CheckInBuffer);
            }
        }

        //sort luggage from return buffer to an open gate
        void SortByReturn()
        {
            Monitor.Enter(Manager.ReturnBuffer);
            try
            {
                if (Manager.ReturnBuffer.InternalLength > 0)
                {

                    //get luggage from return buffer
                    Luggage luggage = Manager.ReturnBuffer.Remove();
                    int index = luggage.Flight.GetGate(luggage.Flight, false);

                    //check if index is valid
                    if (index >= 0)
                    {
                        //check if gate is open
                        Monitor.Enter(Manager.gates[index]);
                        try
                        {
                            //Add luggage to gate buffer if open
                            if (Manager.gates[index].NumLuggage < Manager.gates[index].Flight.MaxLuggage)
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
                        //Debug.WriteLine("no gates available");
                        Manager.ReturnBuffer.Add(luggage);
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.ReturnBuffer);
            }
        }
    }
}
