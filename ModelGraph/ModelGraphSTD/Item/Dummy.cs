using System;

namespace ModelGraphSTD
{
    /// <summary>
    /// Placeholder Dummy Item 
    /// </summary>
    public class Dummy : Item
    {
        internal readonly Guid Guid;
        internal Dummy(Chef owner)
        {
            Owner = owner;
            Trait = Trait.Dummy;
            Guid = new Guid("BB4B121E-9BE4-441B-AEBB-7136889F0143");
        }
    }
}
