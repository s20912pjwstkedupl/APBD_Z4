using System.Data;
using System.Data.SqlClient;
using WebApplication2.Dto;

namespace WebApplication2.Repositories;

public interface IOrderRepository
{
    public Task<int> GetByProductAndCountAndBeforeDateAsync(int productId, int count, DateTime createdAt);
}

public class OrderRepository : IOrderRepository
{
    
    private readonly IConfiguration _configuration;
    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<int> GetByProductAndCountAndBeforeDateAsync(int productId, int count, DateTime createdAt)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand("SELECT IdOrder FROM [Order] WHERE IdProduct = @1 AND Amount = @2 AND FulfilledAt IS NULL AND CreatedAt < @3", connection); 
        command.Parameters.AddWithValue("@1", productId);
        command.Parameters.AddWithValue("@2", count);
        command.Parameters.AddWithValue("@3", createdAt);
        var res = await command.ExecuteScalarAsync();
        if (res is not null)
        {
            return (int)res;
        }

        return -1;
    }
}