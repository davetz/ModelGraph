
namespace ModelGraphSTD
{
    internal class PathParm
    {
        internal string LineColor;   //hex argb color code string

        internal LineStyle LineStyle;
        internal DashStyle DashStyle;

        // connection detail at head end of path
        internal Facet Facet1;
        internal Attach Attach1;
        internal Connect Connect1;

        // connection detail at tail end of path
        internal Facet Facet2;
        internal Attach Attach2;
        internal Connect Connect2;
    }
}
