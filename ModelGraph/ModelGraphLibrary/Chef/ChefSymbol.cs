using System;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        bool TrySetSymbolTopContact(SymbolX sx,  int val)
        {
            if (IsValidEnumValue(typeof(Contact), val))
            {
                sx.TopContact = (Contact)val;
                RefreshDrawing(sx);
            }
            return false;
        }
        bool TrySetSymbolLeftContact(SymbolX sx, int val)
        {
            if (IsValidEnumValue(typeof(Contact), val))
            {
                sx.LeftContact = (Contact)val;
                RefreshDrawing(sx);
            }
            return false;
        }
        bool TrySetSymbolRightContact(SymbolX sx, int val)
        {
            if (IsValidEnumValue(typeof(Contact), val))
            {
                sx.RightContact = (Contact)val;
                RefreshDrawing(sx);
            }
            return false;
        }
        bool TrySetSymbolBottomContact(SymbolX sx, int val)
        {
            if (IsValidEnumValue(typeof(Contact), val))
            {
                sx.BottomContact = (Contact)val;
                RefreshDrawing(sx);
            }
            return false;
        }
    }
}
