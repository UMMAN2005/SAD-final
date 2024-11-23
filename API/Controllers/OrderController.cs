using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;

namespace API.Controllers;

public class OrderController(IOrderRepository orderRepository, IMapper mapper) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetOrders() {
    var orders = await orderRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<OrderGetDto>>(orders), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetOrder(int id) {
    var order = await orderRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<OrderGetDto>(order), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreateOrder(OrderPostDto order) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newOrder = mapper.Map<Order>(order);
    await orderRepository.AddAsync(newOrder);
    var response = new BaseResponse(201, "Created", mapper.Map<OrderGetDto>(newOrder), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteOrder(int id) {
    var order = await orderRepository.GetAsync(x => x.Id == id);
    if (order == null) {
      return NotFound();
    }

    await orderRepository.DeleteAsync(order);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }
}