using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchResults
    {
        public SearchResults()
        {
         
        }

        public SearchResults(List<Shirt> shirts)
        {
            Shirts = shirts;
            SizeCounts = GetSizeCounts(shirts).ToList();
            ColorCounts = GetColorCounts(shirts).ToList();
        }

        public List<Shirt> Shirts { get; set; }
        
        public List<SizeCount> SizeCounts { get; set; }
        
        public List<ColorCount> ColorCounts { get; set; }

        private static IEnumerable<SizeCount> GetSizeCounts(IEnumerable<Shirt> shirts)
        {
            var sizeCounts = shirts.GroupBy(x => x.Size)
                .Select(g =>
                    new SizeCount()
                    {
                        Size = g.Key,
                        Count = g.Count()
                    }).ToList();

            return Size.All.Select(s => new SizeCount()
                { Size = s, Count = sizeCounts.FirstOrDefault(x => x.Size == s)?.Count ?? 0 });
        }

        private static IEnumerable<ColorCount> GetColorCounts(IEnumerable<Shirt> shirts)
        {
            var colorCounts = shirts.GroupBy(x => x.Color)
                .Select(g =>
                    new ColorCount()
                    {
                        Color = g.Key,
                        Count = g.Count()
                    }).ToList();

            return Color.All.Select(c => new ColorCount()
                { Color = c, Count = colorCounts.FirstOrDefault(x => x.Color == c)?.Count ?? 0 });
        }
    }


    public class SizeCount
    {
        public Size Size { get; set; }

        public int Count { get; set; }
    }


    public class ColorCount
    {
        public Color Color { get; set; }

        public int Count { get; set; }
    }
}