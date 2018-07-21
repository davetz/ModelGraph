using System;
using System.Collections.Generic;
using System.Text;

namespace ModelGraphSTD
{
    class GraphHitMargin : Setting<int>
    {
        protected override int DefaultValue => 2;
        protected override bool IsValid(int value) => (value < 1) ? false : (value > 4) ? false : true;
    }
}
