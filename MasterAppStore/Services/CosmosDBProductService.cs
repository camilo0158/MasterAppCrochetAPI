namespace MasterAppStore.Services
{
    using MasterAppStore.Models;
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class CosmosDBProductService : ICosmosDBProductService
    {
        private readonly Container _container;

        public CosmosDBProductService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddProductAsync(Product product)
        {
            await this._container.CreateItemAsync(product);
        }

        public async Task DeleteProductAsync(string id, int partitionKey)
        {
            await _container.DeleteItemAsync<Product>(id, new PartitionKey(partitionKey));
        }

        public async Task<Product> GetProductAsync(string id, int partitionKey)
        {
            try
            {
                ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<Product>> GetProductsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Product>(new QueryDefinition(queryString));
            List<Product> products = new List<Product>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                products.AddRange(response.ToList());
            }
            return products;
        }

        public async Task UpdateProductAsync(string id, int partitionKey, Product product)
        {
            await _container.ReplaceItemAsync<Product>(product, id, new PartitionKey(partitionKey));
        }
    }
}
