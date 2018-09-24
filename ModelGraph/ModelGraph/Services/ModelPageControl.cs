using ModelGraph.Controls;
using ModelGraphSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace ModelGraph.Services
{
    public sealed class ModelPageControl : IPageControl
    {
        public IModelControl ModelControl { get; private set; }
        public RootModel RootModel { get; private set; }
        public CoreDispatcher Dispatcher { get; set; }

        public ModelPageControl(RootModel rootModel)
        {
            RootModel = rootModel;
            rootModel.PageControl = this;
        }

        public async void Dispatch(UIRequest rq)
        {
            await ModelPageService.Current.Dispatch(rq, Dispatcher);
        }
    }
}
