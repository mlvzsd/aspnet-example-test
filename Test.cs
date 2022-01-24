using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using TodoAPI.Controllers;

namespace ControllerAPI.Tests;

public class Test
{
    [Fact]
    public async Task GetTodos_ReturnsTodos()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            var expectedValues = TodoContext.GetSeedingTodoItems();

            //Act
            var result = await new TodoItemsController(db).GetTodoItems();
            
            //Assert
            var actualValues = Assert.IsAssignableFrom<List<TodoItemDTO>>(result.Value);
            Assert.Equal(
                expectedValues.OrderBy(m => m.Id).Select(m => m.Name),
                actualValues.OrderBy(m => m.Id).Select(m => m.Name)
            );
            
        }
    }
    
    [Fact]
    public async Task GetTodo_ReturnTodo_WhenFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 1;
            var expetedValue = TodoContext.GetSeedingTodoItems().Where(t => t.Id == id).ElementAt(0);
            
            //Act
            var aux = await new TodoItemsController(db).GetTodoItem(id);
            var actualValue = aux.Value;
            
            //Assert
            Assert.Equal(expetedValue.Name, actualValue?.Name);
        }
    }
    
    [Fact]
    public async Task GetTodo_ReturnNotFound_WhenNotFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = -1;
            var controller = new TodoItemsController(db);
            var expectedValue = controller.NotFound();
            
        
            //Act
            var actualValue = (StatusCodeResult)(await controller.GetTodoItem(id)).Result;
            
        
            //Assert
            Assert.Equal(expectedValue.StatusCode, actualValue?.StatusCode);

        }
    }

    [Fact]
    public async Task PutTudo_SaveChanges_WhenFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 1;
            var expectedValue = new TodoItemDTO() { Id = id, Name = "Fazer arroz", IsComplete = true };
            var controller = new TodoItemsController(db);
            var expectedResponse = controller.NoContent();
            
            //Act
            var actualResponse = (StatusCodeResult) await controller.PutTodoItem(id, expectedValue);
            var actualValue = db.TodoItems.Find((long)id);
            
            //Assert
            Assert.Equal(expectedResponse.StatusCode, (actualResponse).StatusCode);
            Assert.Equal(expectedValue.Name, actualValue?.Name);
            Assert.Equal(expectedValue.IsComplete, actualValue?.IsComplete);
        }
    }

    [Fact]
    public async Task PutTodo_ReturnsNotFound_WhenNotFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 10;
            var todoDummy = new TodoItemDTO() { Id = id, Name = "Fazer arroz", IsComplete = true };
            var controller = new TodoItemsController(db);
            var expectedResponse = controller.NotFound();
            
            //Act
            var actualResponse = (StatusCodeResult)await controller.PutTodoItem(id, todoDummy);
            
            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
        }
    }

    [Fact]
    public async Task PutTodo_ReturnBadRequest_WhenInvalidId()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 1;
            var todoDummy = new TodoItemDTO() { Id = id+1, Name = "Fazer arroz", IsComplete = true };
            var controller = new TodoItemsController(db);
            var expectedResponse = controller.BadRequest();
            
            //Act
            var actualResponse = (StatusCodeResult)await controller.PutTodoItem(id, todoDummy);
            
            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
        }    
    }
    
    public async Task DeleteTodo_DeleteTodo_WhenFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 1;
            var controller = new TodoItemsController(db);
            var expectedResponse = controller.NoContent();
            
            //Act
            var actualResponse = (StatusCodeResult)await controller.DeleteTodoItem(id);
            var todos = db.TodoItems.ToList();
            
            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
            Assert.False(todos.Select(t => t.Id).Contains(id));
        }
    }
    public async Task DeleteTodo_ReturnsNotFound_WhenNotFound()
    {
        using(var db = new TodoContext(Utilites.TestDbContextOptions()))
        {
            //Arrange
            db.SeedDb();
            int id = 10;
            var controller = new TodoItemsController(db);
            var expectedResponse = controller.NotFound();
            
            //Act
            var actualResponse = (StatusCodeResult)await controller.DeleteTodoItem(id);
            
            //Assert
            Assert.Equal(expectedResponse.StatusCode, actualResponse.StatusCode);
        }
    }
}
