using MasterAppStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterAppStore.Services
{
    public interface ICosmosDBProductService
    {
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(string id, int partitionKey);
        Task<Product> GetProductAsync(string id, int partitionKey);
        Task<List<Product>> GetProductsAsync(string queryString);
        Task UpdateProductAsync(string id, int partitionKey, Product product);
    }
}