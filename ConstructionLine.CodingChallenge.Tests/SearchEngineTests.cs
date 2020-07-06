using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        private List<Shirt> _shirts;
        private SearchEngine _searchEngine;

        [SetUp]
        public void SetUp()
        {
            _shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Red - Medium", Size.Medium, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            _searchEngine = new SearchEngine(_shirts);
        }

        [Test]
        public void Test()
        {
            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red},
                Sizes = new List<Size> {Size.Small}
            };

            var results = _searchEngine.Search(searchOptions);

            Assert.That(results.Shirts.Count, Is.EqualTo(1));
            AssertResults(results, searchOptions);
        }

        [Test]
        public void Test_SearchWithColorsOnly()
        {
            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> { Color.Red, Color.Blue }
            };

            var results = _searchEngine.Search(searchOptions);

            Assert.That(results.Shirts.Count, Is.EqualTo(3));
            AssertResults(results, searchOptions);
        }

        [Test]
        public void Test_SearchWithSizesOnly()
        {
            var searchOptions = new SearchOptions
            {
                Sizes = new List<Size> { Size.Medium, Size.Large }
            };

            var results = _searchEngine.Search(searchOptions);

            Assert.That(results.Shirts.Count, Is.EqualTo(3));
            AssertResults(results, searchOptions);
        }
        
        [Test]
        public void Test_SearchWithoutAnyOptions()
        {
            var searchOptions = new SearchOptions();

            var results = _searchEngine.Search(searchOptions);

            Assert.That(results.Shirts.Count, Is.EqualTo(4));
            AssertResults(results, searchOptions);
        }

        private void AssertResults(SearchResults results, SearchOptions searchOptions)
        {
            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(_shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(_shirts, searchOptions, results.ColorCounts);
        }
    }
}
