using System.Collections.Generic;

using Windows.UI.Xaml.Media;

namespace TemplateStudioUWP.Models
{
    public class SharedDataStorageItemsModel : SharedDataModelBase
    {
        public List<ImageSource> Images { get; } = new List<ImageSource>();

        public SharedDataStorageItemsModel()
            : base()
        {
        }
    }
}
