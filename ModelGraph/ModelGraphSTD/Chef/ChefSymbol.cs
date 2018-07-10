using System;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        bool TrySetSymbolTopContact(SymbolX sx, int val)
        {
            sx.TopContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolLeftContact(SymbolX sx, int val)
        {
            sx.LeftContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolRightContact(SymbolX sx, int val)
        {
            sx.RightContact = (Contact)val;
            return true;
        }
        bool TrySetSymbolBottomContact(SymbolX sx, int val)
        {
            sx.BottomContact = (Contact)val;
            return true;
        }
    }
}
