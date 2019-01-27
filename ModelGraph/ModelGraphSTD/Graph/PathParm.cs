
namespace ModelGraphSTD
{
    internal class PathParm
    {
        internal string LineColor;   //hex argb color code string

        internal LineStyle LineStyle;
        internal DashStyle DashStyle;

        // connection detail at head end of path
        internal Target Target1;
        internal Connect Connect1; //[DELETE]
        internal Facet Facet1;

        // connection detail at tail end of path
        internal Target Target2;
        internal Connect Connect2; //[DELETE]
        internal Facet Facet2;
    }
}
