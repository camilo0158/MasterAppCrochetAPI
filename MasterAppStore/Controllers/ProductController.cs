namespace MasterAppStore.Controllers
{
    using MasterAppStore.Models;
    using MasterAppStore.Services;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ICosmosDBProductService _productService;

        public ProductController(ICosmosDBProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAsync()
        {
            string query = "SELECT * FROM c";
            List<Product> products =  await _productService.GetProductsAsync(query);
            return products;
        }

        [HttpGet("{id}/{category}")]
        public async Task<ActionResult<Product>> GetProductAsync([Bind("Id, Category")] string id, string category)
        {
            Product product =  await _productService.GetProductAsync(id, int.Parse(category));
            if(product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateAsync([Bind("Category, Description, Quantity, Color, Size, Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid().ToString();
                await _productService.AddProductAsync(product);
            }
            return product;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([Bind("Id, Category, Description, Quantity, Color, Size, Price")] Product product)
        {
            
            Product productToUpdate =  await _productService.GetProductAsync(product.Id, (int)product.Category);
            if(productToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(productToUpdate.Id, (int)productToUpdate.Category, product);
            }
            return NoContent();
        }

        [HttpDelete("{id}/{category}")]
        public async Task<ActionResult> DeleteAsync([Bind("Id, Category")] string id, string category)
        {
            Product productToDelete = await _productService.GetProductAsync(id, int.Parse(category));
            if (productToDelete == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(productToDelete.Id, (int)productToDelete.Category);
            return NoContent();
        }
    }
}
