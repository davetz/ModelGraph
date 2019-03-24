using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ModelGraphSTD
{
    internal static class Layout
    {
        #region Offset  =======================================================
        internal static int Offset(int index, int count)
        {/*
                The index is 0 based, the totalCount is 1's based 
                for examle:    0,  1,  2,  3,  4 with the totalCount = 5
                    offset:   -4, -2,  0,  2,  4  
                             . . . . . | . . . . . 
                 or examle:  0,  1,  2,  3,  4,  5 with the totalCount = 6
                    offset: -5, -3, -1,  1,  3,  5
           
                The diference between succesive offset values is always 2
         */
            return 2 * index - (count - 1);
        }
        #endregion
    }
}