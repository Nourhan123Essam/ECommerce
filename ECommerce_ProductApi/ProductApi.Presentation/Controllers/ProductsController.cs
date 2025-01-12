using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var prducts = await productInterface.GetAllAsync();
            if (prducts.Any())
            {
                //convert data from entity to DTO
                var (_, list) = ProductConversion.FromEntity(null, prducts);
                return list.Any() ? Ok(list) : NotFound("No products found");
            }
            else
            {
                return NotFound("No products detected in the database");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product == null)
            {
                return NotFound("Product requested not found");
            }

            //convert from entity to DTO
            var (_product, _) = ProductConversion.FromEntity(product, null);
            return _product is not null ? Ok(_product) : NotFound("Product not found");            
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
