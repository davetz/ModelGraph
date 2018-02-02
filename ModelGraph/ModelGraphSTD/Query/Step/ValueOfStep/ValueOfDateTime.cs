﻿using System;

namespace ModelGraphSTD
{
    internal abstract class ValueOfDateTime : ValueOfStep<DateTime>
    {
        internal override ValType ValType => ValType.DateTime;

        internal override bool AsBool() => false;
        internal override long AsLong() => 0;
        internal override double AsDouble() => 0;
        internal override string AsString() => GetVal().ToString();
        internal override DateTime AsDateTime() => GetVal();
    }
}