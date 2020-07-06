using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;
        }

        public SearchResults Search(SearchOptions options)
        {
            // Based on the given tests, I am assuming that options is never going to be passed as null so not covering that corner-case here

            var sizeCount = InitializeSizeCount();
            var colorCount = InitializeColorCount();
            var filteredShirts = new List<Shirt>();

            var localLockObject = new object();

            Parallel.ForEach(
                _shirts,
                () => (
                        shirts: new List<Shirt>(),
                        sizeCount: InitializeSizeCount(),
                        colorCounts: InitializeColorCount()
                    ),
                (shirt, state, tuple) =>
                {
                    if (ShirtSizeMatches(shirt, options.Sizes) && ShirtColorMatches(shirt, options.Colors))
                    {
                        tuple.shirts.Add(shirt);
                        tuple.sizeCount[shirt.Size.Id].Count++;
                        tuple.colorCounts[shirt.Color.Id].Count++;
                    }

                    return tuple;
                },
                (finalResult) =>
                {
                    lock (localLockObject)
                    {
                        var (shirts, sizeCounts, colorCounts) = finalResult;

                        filteredShirts.AddRange(shirts);

                        foreach (var (key, value) in sizeCounts)
                        {
                            sizeCount[key].Count += value.Count;
                        }

                        foreach (var (key, value) in colorCounts)
                        {
                            colorCount[key].Count += value.Count;
                        }
                    }
                }
            );

            return new SearchResults
            {
                Shirts = filteredShirts.ToList(),
                SizeCounts = sizeCount.Select(s => s.Value).ToList(),
                ColorCounts = colorCount.Select(c => c.Value).ToList()
            };
        }

        private static bool ShirtSizeMatches(Shirt shirt, IReadOnlyCollection<Size> optionsSizes)
        {
            return !FilterBySize(optionsSizes) || optionsSizes.SingleOrDefault(size => shirt.Size == size) != null;
        }

        private static bool ShirtColorMatches(Shirt shirt, IReadOnlyCollection<Color> optionsColors)
        {
            return !FilterByColor(optionsColors) || optionsColors.SingleOrDefault(color => shirt.Color == color) != null;
        }

        private static bool FilterBySize(IEnumerable<Size> optionsSizes)
        {
            return optionsSizes.Any();
        }

        private static bool FilterByColor(IEnumerable<Color> optionsColors)
        {
            return optionsColors.Any();
        }

        private static Dictionary<Guid, SizeCount> InitializeSizeCount()
        {
            return Size.All.Select(s => new SizeCount() { Size = s }).ToDictionary(color => color.Size.Id);
        }

        private static Dictionary<Guid, ColorCount> InitializeColorCount()
        {
            return Color.All.Select(c => new ColorCount() { Color = c }).ToDictionary(color => color.Color.Id);
        }

        #region AsParallel

        public SearchResults SearchAsParallel(SearchOptions options)
        {
            var filteredShirts = _shirts.AsParallel().Where(shirt =>
                (!FilterBySize(options.Sizes) || options.Sizes.Any(s => s == shirt.Size)) &&
                (!FilterByColor(options.Colors) || options.Colors.Any(c => c == shirt.Color))).ToList();

            return new SearchResults(filteredShirts);
        }

        #endregion
    }
}