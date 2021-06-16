using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    public class Queue
    {
        private int internalLength;
        private int length;

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

        private Luggage[] buffer;

        public Luggage[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        public Queue(int _length)
        {
            this.buffer = new Luggage[_length];
            this.length = _length;
            this.internalLength = 0;
        }

        public void Add(Luggage luggage)
        {
            //  Console.WriteLine(luggage.Flight.FlightNumber);
            //move all elements in array
            if (internalLength > 0)
            {
               

            }

            buffer[internalLength] = luggage;
            internalLength++;


        }

        public Luggage Remove()
        {
            Luggage luggage = buffer[0];
            for (int i = 0; i < internalLength; i++)
            {
                buffer[i] = buffer[i + 1];
            }
            internalLength--;
            return luggage;
        }
    }
}
