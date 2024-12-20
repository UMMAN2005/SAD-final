﻿using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;

namespace API.Controllers;

public class ProductController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetProducts() {
    var products = await productRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<ProductGetDto>>(products), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetProduct(int id) {
    var product = await productRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<ProductGetDto>(product), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreateProduct(ProductPostDto product) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newProduct = mapper.Map<Product>(product);

    var category = await categoryRepository.GetAsync(x => x.Id == product.CategoryId);
    if (category == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

    await productRepository.AddAsync(newProduct);
    var response = new BaseResponse(201, "Created", mapper.Map<ProductGetDto>(newProduct), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteProduct(int id) {
    var product = await productRepository.GetAsync(x => x.Id == id);
    if (product == null) {
      return NotFound();
    }

    await productRepository.DeleteAsync(product);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }

  [HttpGet("category/{id:int}")]
  public async Task<IActionResult> GetProductsByCategory(int id) {
    var products = await productRepository.GetAllAsync(x => x.CategoryId == id);
    var response = new BaseResponse(200, "Success", mapper.Map<List<ProductGetDto>>(products), []);
    return Ok(response);
  }
}