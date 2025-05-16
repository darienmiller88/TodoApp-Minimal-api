using System.Collections.Generic;
using System.Threading;
using api.v1.Models;
using MongoDB.Driver;
using Moq;

namespace Tests;
public static class MockTodoService{
    public static Mock<IMongoCollection<Todo>> GetMockTodoService(List<Todo> dummyTodos){
        //First, create a new cursor object, which will be used by our fake mongo object to look up the mongo cluster
        var mockCursor = new Mock<IAsyncCursor<Todo>>();

        //Setup the cursor to return a list of todos as the dummy data.
        mockCursor.Setup(_ => _.Current).Returns(dummyTodos);
        
        //Finally, Set up the sequence so that at most, one batch of data is retrieved, and no more.
        mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
    
        var mockCollection = new Mock<IMongoCollection<Todo>>();
        mockCollection.Setup(x => x.FindAsync(
            It.IsAny<FilterDefinition<Todo>>(),
            It.IsAny<FindOptions<Todo, Todo>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(mockCursor.Object);

        return mockCollection;
    }
}