using ModelGraph.Controls;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGraph.Services
{
    public sealed class ModelPageControl : IPageControl
    {
        public IModelControl ModelControl { get; private set; }
        public RootModel RootModel { get; private set; }

        public ModelPageControl(RootModel rootModel)
        {
            RootModel = rootModel;
            rootModel.PageControl = this;
        }

        public void Dispatch(UIRequest request)
        {
        }

        public void SetActualSize()
        {
        }

    }
}
