using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoAPI.Models;

namespace ControllerAPI.Tests
{
    public class Utilites
    {
        public static DbContextOptions<TodoContext> TestDbContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            
            var builder = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);
            
            return builder.Options;
        }
    }
}