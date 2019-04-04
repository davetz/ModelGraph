using System;
using System.Collections.Generic;
using System.Xml;

namespace ModelGraphSTD
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
                { _nodeLabelingProperty, _labelingEnum },
                { _nodeResizingProperty, _resizingEnum },
                { _nodeBarWidthProperty, _barWidthEnum },

                { _edgeFacet1Property, _facetEnum },
                { _edgeFacet2Property, _facetEnum },

                { _symbolXAttachProperty, _attatchEnum },

                { _queryXLineStyleProperty, _lineStyleEnum },
                { _queryXDashStyleProperty, _dashStyleEnum },

                { _queryXFacet1Property, _facetEnum },
                { _queryXFacet2Property, _facetEnum },

                { _computeXCompuTypeProperty, _computeTypeEnum },
                { _computeXResultsProperty, _computeResultsEnum },
                { _computeXSortingProperty, _computeSortingEnum },
            };
        }
    }
}
