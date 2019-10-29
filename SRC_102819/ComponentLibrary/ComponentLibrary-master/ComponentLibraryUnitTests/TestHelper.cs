using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests
{
    public class TestHelper
    {
        public static void MockCollectionWithExisting<T>(Mock<IMongoCollection<T>> mockCollection, params T[] data)
        {
            if (mockCollection == null)
            {
                mockCollection = new Mock<IMongoCollection<T>>();
            }
            MockCollectionWithExistingList(mockCollection, data?.Select(datum => datum == null ? new List<T>() : new List<T> { datum }).ToArray());
        }

        private static IAsyncCursor<T> MockCursor<T>(IEnumerable<T> list)
        {
            var mockCursor = new Mock<IAsyncCursor<T>>();
            var enumerator = list.GetEnumerator();
            mockCursor.Setup(m => m.Current).Returns(() => new List<T> { enumerator.Current });
            mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
            return mockCursor.Object;
        }

        public static void MockCollectionWithExistingList<T>(Mock<IMongoCollection<T>> mockCollection, params IEnumerable<T>[] lists)
        {
            var setupSequence = mockCollection.SetupSequence(m => m.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                default(CancellationToken)));
            if (lists == null)
            {
                setupSequence.ReturnsAsync(MockCursor(new List<T>()));
            }
            else
            {
                lists.Aggregate(setupSequence, (s, list) => s.ReturnsAsync(MockCursor(list)));
            }
            mockCollection.Setup(m => m.CountAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<CountOptions>(),
                default(CancellationToken))
                ).ReturnsAsync(lists?.First().Count() ?? 0);
        }

        public static void MockCollectionToReturnAllListItems<T>(Mock<IMongoCollection<T>> mockCollection, IEnumerable<T> list)
        {
            var setupSequence = mockCollection.SetupSequence(m => m.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                default(CancellationToken)));

            setupSequence.ReturnsAsync(MockCursor(list));

            mockCollection.Setup(m => m.CountAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<CountOptions>(),
                default(CancellationToken))
            ).ReturnsAsync(list.Count());
        }

        public static void MockCollectionWithMultipleResponseForFind<T>(Mock<IMongoCollection<T>> mockCollection, T data, params IEnumerable<T>[] lists)
        {
            var mockCursor = new Mock<IAsyncCursor<T>>();
            mockCursor.Setup(m => m.Current).Returns(() => new List<T>());
            var setupSequence = mockCollection.SetupSequence(m => m.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                default(CancellationToken)));

            setupSequence.ReturnsAsync(mockCursor.Object);

            if (lists == null)
                setupSequence.ReturnsAsync(MockCursor(new List<T>()));
            else
                lists.Aggregate(setupSequence, (s, list) => s.ReturnsAsync(MockCursor(list)));
            
            mockCollection.Setup(m => m.CountAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<CountOptions>(),
                default(CancellationToken))
                ).ReturnsAsync(lists?.First().Count() ?? 0);
        }

        public static void MockCollectionWithExistingAndFindReturnEmptyList<T>(Mock<IMongoCollection<T>> mockCollection, T data)
        {
            if (mockCollection == null)
            {
                mockCollection = new Mock<IMongoCollection<T>>();
            }
            var mockCursor = new Mock<IAsyncCursor<T>>();
            var masterDataListDaos = data == null ? new List<T>() : new List<T> { data };
            var enumerator = masterDataListDaos.GetEnumerator();
            mockCursor.Setup(m => m.Current).Returns(() => new List<T>());
            mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
            mockCollection.Setup(m => m.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                default(CancellationToken)))
                .ReturnsAsync(mockCursor.Object);
        }
    }
}