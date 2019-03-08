
namespace ModelGraphSTD
{
    internal class PathParm
    {
        internal string LineColor;   //hex argb color code string

        internal LineStyle LineStyle;
        internal DashStyle DashStyle;

        // connection detail at head end of path
        internal Target Target1;
        internal Facet Facet1;

        // connection detail at tail end of path
        internal Target Target2;
        internal Facet Facet2;
    }
}
