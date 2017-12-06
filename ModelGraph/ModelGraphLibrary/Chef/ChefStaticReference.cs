using System;
using System.Collections.Generic;
using System.Xml;

namespace ModelGraphLibrary
{/*

 */
    public partial class Chef
    {
        private Dictionary<Property, EnumZ> Property_Enum;

        private void InitializeReferences()
        {
            // static enum properties
            Property_Enum = new Dictionary<Property, EnumZ>
            {
                { _columnXTypeOfProperty, _valueTypeEnum },

                { _relationXPairingProperty, _pairingEnum },

                { _nodeOrientationProperty, _orientationEnum },
                { _nodeFlipRotateProperty, _flipRotateEnum },
                { _nodeLabelingProperty, _labelingEnum },
                { _nodeResizingProperty, _resizingEnum },
                { _nodeBarWidthProperty, _barWidthEnum },

                { _edgeFace1Property, _sideEnum },
                { _edgeFace2Property, _sideEnum },
                { _edgeGnarl1Property, _facetEnum },
                { _edgeGnarl2Property, _facetEnum },
                { _edgeConnect1Property, _connectEnum },
                { _edgeConnect2Property, _connectEnum },

                { _symbolXTopContactProperty, _contactEnum },
                { _symbolXLeftContacttProperty, _contactEnum },
                { _symbolXRightContactProperty, _contactEnum },
                { _symbolXBottomContactProperty, _contactEnum },

                { _queryXConnect1Property, _connectEnum },
                { _queryXConnect2Property, _connectEnum },

                { _computeXCompuTypeProperty, _computeTypeEnum },
                { _computeXNumericSetProperty, _numericSetEnum }
            };
        }
    }
}
