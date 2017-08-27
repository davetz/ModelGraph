using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraphLibrary
{
    public abstract class Step
    {
        protected Step[] Inputs;      // a step may have zero or more inputs
        protected NativeType InType;  // expected native data type of inputs
        protected NativeType OutType; // native output data type
        protected StepFlags StepFlags;  // 
        protected byte MinArgs;       // minimum number of input arguments
        protected byte MaxArgs;       // maximum numbero of input arguments
    }
}
