using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;

namespace API.Controllers;

public class CategoryController(ICategoryRepository categoryRepository, IMapper mapper) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetCategories() {
    var categories = await categoryRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<CategoryGetDto>>(categories), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetCategory(int id) {
    var category = await categoryRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<CategoryGetDto>(category), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreateCategory(CategoryPostDto category) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newCategory = mapper.Map<Category>(category);
    await categoryRepository.AddAsync(newCategory);
    var response = new BaseResponse(201, "Created", mapper.Map<CategoryGetDto>(newCategory), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteCategory(int id) {
    var category = await categoryRepository.GetAsync(x => x.Id == id);
    if (category == null) {
      return NotFound();
    }

    await categoryRepository.DeleteAsync(category);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }
}