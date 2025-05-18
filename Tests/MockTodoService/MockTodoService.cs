using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.v1.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace Tests;
public static class MockTodoService{
    public static Mock<IMongoCollection<Todo>> GetMockTodoService(List<Todo> dummyTodos)
    {
        //First, create a new cursor object, which will be used by our fake mongo object to look up the mongo cluster
        // var mockCursor = new Mock<IAsyncCursor<Todo>>();

        // //Setup the cursor to return a list of todos as the dummy data.
        // mockCursor.Setup(_ => _.Current).Returns(dummyTodos);

        // //Finally, Set up the sequence so that at most, one batch of data is retrieved, and no more.
        // mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        // var mockCollection = new Mock<IMongoCollection<Todo>>();
        // mockCollection.Setup(x => x.FindAsync(
        //     It.IsAny<FilterDefinition<Todo>>(),
        //     It.IsAny<FindOptions<Todo, Todo>>(),
        //     It.IsAny<CancellationToken>())
        // ).ReturnsAsync(mockCursor.Object);

        // return mockCollection;
        
        var mockCollection = new Mock<IMongoCollection<Todo>>();

        mockCollection.Setup(collection => collection.FindAsync(
            It.IsAny<FilterDefinition<Todo>>(),
            It.IsAny<FindOptions<Todo, Todo>>(),
            It.IsAny<CancellationToken>())
        ).Returns<FilterDefinition<Todo>, FindOptions<Todo, Todo>, CancellationToken>(
            (filter, options, token) => {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<Todo>();

            var renderedFilter = filter.Render(
                new RenderArgs<Todo>(documentSerializer, serializerRegistry)
            );

            // Extract the ID being searched (assumes it's an ID-based query)
            string? filterId = renderedFilter.GetValue("_id", null)?.AsString;
            List<Todo>? matched = string.IsNullOrEmpty(filterId) ? dummyTodos : dummyTodos.Where(t => t.Id == filterId).ToList();

            var mockCursor = new Mock<IAsyncCursor<Todo>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            mockCursor.Setup(c => c.Current).Returns(matched);

            return Task.FromResult(mockCursor.Object);
        });

        return mockCollection;
    }
}