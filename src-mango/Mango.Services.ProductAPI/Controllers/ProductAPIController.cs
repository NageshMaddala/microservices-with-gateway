using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> Products = _db.Products.ToList();
                //_response.Result = Products;
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(Products);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product Product = _db.Products.First(c => c.ProductId == id);
                //_response.Result = Product;

                // return back the dto instead of entity
                // converts Product to ProductDTO
                _response.Result = _mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto ProductDto)
        {
            try
            {
                // convert dto to Product entity
                Product obj = _mapper.Map<Product>(ProductDto);

                // add record to Product table
                _db.Products.Add(obj);
                // to persist change, it runs query here
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="ProductDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto UpdateProduct([FromBody] ProductDto ProductDto)
        {
            try
            {
                // convert dto to Product entity
                Product obj = _mapper.Map<Product>(ProductDto);

                // EF would update Product based on Product id
                _db.Products.Update(obj);

                // to persist change, it runs query here
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(c => c.ProductId == id);
                _db.Products.Remove(obj);

                // to persist change, it runs query here
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
