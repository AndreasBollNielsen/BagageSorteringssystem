using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BagageSorteringssystem
{
    public class Queue
    {
        //fields
        private int internalLength;
        private int length;
        private Luggage[] buffer;

        //properties
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public int InternalLength
        {
            get { return internalLength; }
            set { internalLength = value; }
        }
        public Luggage[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        //constructor
        public Queue(int _length)
        {
            this.buffer = new Luggage[_length];
            this.length = _length;
            this.internalLength = 0;
        }


        //add luggage element to buffer
        public void Add(Luggage luggage)
        {
            //buffer warning if size exceeds length of array
            if (internalLength >= length)
            {
                Debug.WriteLine("buffer exeeded ");
                return;
            }

            //add element to buffer & increase index number
            buffer[internalLength] = luggage;
            internalLength++;
        }

        //remove luggage element from buffer
        public Luggage Remove()
        {
            Luggage luggage = buffer[0];

            //move all elements to the left
            for (int i = 0; i < internalLength; i++)
            {
                buffer[i] = buffer[i + 1];
            }

            //decrease index
            internalLength--;
            return luggage;
        }

        //return a sample of the buffer at index 0
        public FlightPlan Inspect()
        {
            Luggage luggage = buffer[0];
            return luggage.Flight;
        }
    }
}
