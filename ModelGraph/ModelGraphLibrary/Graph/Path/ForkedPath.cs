using System;

namespace ModelGraph.Internals
{/*
 */
    public class ForkedPath : Path
    {
        internal Path Path1;
        internal Path[] Paths;

        #region Constructor  ==================================================
        internal ForkedPath(Graph owner, Path path1, Path[] paths)
        {
            Owner = owner;
            Trait = Trait.ForkedPath;
            IsRadial = true;

            Path1 = path1;
            Paths = paths;

            owner.Add(this);
        }
        #endregion

        #region Properties/Methods  ===========================================
        internal override int Count => 2;
        internal override Item[] Items => GetItems();
        private Item[] GetItems()
        {
            var len = Paths.Length;
            var items = new Item[len];
            for (int i = 0; i < len; i++) { items[i] = Paths[i]; }
            return items;
        }
        internal override Query Query { get { return Path1.Query; } }
        internal override Item Head { get { return IsReversed ? Path1.Tail : Path1.Head; } }
        internal override Item Tail { get { return IsReversed ? Path1.Head : Path1.Tail; } }

        internal override double Width { get { return GetWidth(); } }
        internal override double Height { get { return GetHeight(); } }

        private double GetWidth()
        {
            double maxWidth = 0;
            foreach (var path in Paths)
            {
                var width = path.Width;
                if (width > maxWidth) maxWidth = width;
            }
            return maxWidth + Path1.Width;
        }
        private double GetHeight()
        {
            double height = 0;
            foreach (var path in Paths)
            {
                height += path.Height;
            }
            var temp = Path1.Height;
            return (temp > height) ? temp : height;
        }
        #endregion
    }
}
