using System.Data;
using System.Data.SqlClient;

namespace WebApplication2.Repositories;

public interface IProductRepository
{
    public Task<bool> HasProductWithIdAsync(int id);
    public Task<decimal> GetPriceByIdAsync(int id);
}

public class ProductRepository : IProductRepository
{
    
    private readonly IConfiguration _configuration;
    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> HasProductWithIdAsync(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand("select * from Product where IdProduct = @1", connection); 
        command.Parameters.AddWithValue("@1", id);
        if (await command.ExecuteScalarAsync() is null)
        {
            return false;
        }
        return true;
    }
    
    public async Task<decimal> GetPriceByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @1";
        command.Parameters.AddWithValue("@1", id);
        await connection.OpenAsync();
        
        var result = await command.ExecuteScalarAsync();
        if (result is not null)
        {
            return (decimal)result;
        }
        throw new Exception("Product price was not found");
        
    }
}