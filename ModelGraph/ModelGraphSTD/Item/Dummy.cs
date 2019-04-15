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
    /// <summary>
    /// Owner of any import file read errors
    /// </summary>
    public class ImportBinaryReader : Item
    {
        internal ImportBinaryReader(Chef owner)
        {
            Owner = owner;
            Trait = Trait.ImportBinaryReader;
        }
    }
    /// <summary>
    /// Owner of any export file write errors
    /// </summary>
    public class ExportBinaryWriter : Item
    {
        internal ExportBinaryWriter(Chef owner)
        {
            Owner = owner;
            Trait = Trait.ExportBinaryWriter;
        }
    }
}
