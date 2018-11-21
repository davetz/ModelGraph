using System;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        bool TrySetSymbolTopContact(SymbolX sx, int val)
        {
            sx.NorthContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolLeftContact(SymbolX sx, int val)
        {
            sx.WestContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolRightContact(SymbolX sx, int val)
        {
            sx.EastContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolBottomContact(SymbolX sx, int val)
        {
            sx.SouthContact = (Contact)val;
            return true;
        }
    }
}
