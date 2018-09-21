using System;
using System.Runtime.InteropServices;

using Windows.ApplicationModel.Resources;

namespace ModelGraph.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _resLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }

        public static Func<string, string> GetLocalizer()
        {
            return (s) => _resLoader.GetString(s);
        }
    }
}
