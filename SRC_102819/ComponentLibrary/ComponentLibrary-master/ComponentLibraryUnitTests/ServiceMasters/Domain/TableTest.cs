using System;
using System.Collections.Generic;
using System.Linq;
using ComponentLibrary.MaterialMasters.Domain;
using FluentAssertions;
using Xunit;

namespace ComponentLibraryUnitTests.Materials.Domain
{
    public class TableTest
    {
        [Fact]
        public void Data_Returns_Data_Which_Is_Header_And_Cells_Merged()
        {
            IEnumerable<ICell> headerCells = new ICell[]
            {
                new TextCell("Sl. No"), new TextCell("Work Description"), new ImageCell("http://imageurl.com"),
                new TextCell("Remarks")
            };
            IEnumerable<Entry> entries = new[]
            {
                new Entry(new ICell[]
                {new TextCell("1"), new TextCell("Material is parkted completely with corner protection")}),
                new Entry(new ICell[]
                {new TextCell("2"), new TextCell("Material 2 is parkted completely with corner protection")})
            };

            var header = new List<Entry> {new Entry(headerCells)};
            var table = new Table(headerCells, entries);
            table.Content().Count().Should().Be(3);
            table.Content().FirstOrDefault().Equals(headerCells);
            table.Content().LastOrDefault().Equals(new Entry(new ICell[]
            {new TextCell("2"), new TextCell("Material 2 is parkted completely with corner protection")})
                );
        }

        [Fact]
        public void NewTable_ContentIsEmpty_RaiseInvalidArgumentException()
        {
            Action act = () => new Table(new List<ICell> {new TextCell("")}, new List<Entry>());
            act.ShouldThrow<ArgumentException>()
                .WithMessage("Table cannot be empty.");
        }

        [Fact]
        public void NewTable_ContentOrContentAndHeadersAreNull_RaiseInvalidArgumentException()
        {
            Action act = () => new Table(null, null);
            act.ShouldThrow<ArgumentException>()
                .WithMessage("Table cannot be empty.");
        }

        [Fact]
        public void NewTable_HeaderLengthIsLessThanMaxEntryLength_RaiseInvalidArgumentException()
        {
            IEnumerable<ICell> headerCells = new ICell[]
            {
                new TextCell("Sl. No")
            };
            IEnumerable<Entry> entries = new[]
            {
                new Entry(new ICell[]
                {new TextCell("1")}),
                new Entry(new ICell[]
                {new TextCell("1"), new TextCell("Sattar")})
            };
            Action act = () => new Table(headerCells, entries);
            act.ShouldThrow<ArgumentException>()
                .WithMessage("Entry size cannot exceed header size.");
        }

        [Fact]
        public void NewTable_HeadersIsEmpty_RaiseInvalidArgumentException()
        {
            Action act = () => new Table(new List<ICell>(), new List<Entry> {new Entry(new List<ICell>())});
            act.ShouldThrow<ArgumentException>()
                .WithMessage("Table cannot be empty.");
        }
    }
}