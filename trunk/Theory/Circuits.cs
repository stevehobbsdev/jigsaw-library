using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class Circuits
    {
        public class Pin 
        {            
        }

        public class InputPin : Pin
        {
        }

        public class OutputPin : Pin
        {
        }

        public class Circuit
        {
            public List<InputPin> InputPins = new List<InputPin>();
            public List<OutputPin> OutputPins = new List<OutputPin>();
            public Circuit(int inputs, int outputs)
            {
                for (int i=0; i < inputs; ++i)
                    InputPins.Add(new InputPin());
                for (int i = 0; i < outputs; ++i)
                    OutputPins.Add(new OutputPin());
            }            
        }

        public class BinaryLogicGate : Circuit
        {
            private Func<bool, bool, bool> func;
            public BinaryLogicGate(Func<bool, bool, bool> f)
                : base(2, 1)
            {
            }
        }

        public class Nand : Circuit
        {
            Nand()
                : base(2, 1)
            { }
        }
    }


}
