using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace API.Controllers;

public class OrderController(IOrderRepository orderRepository, IMapper mapper, IHttpContextAccessor accessor, UserManager<AppUser> userManager) : BaseApiController {
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

    var token = accessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (token == null) return Unauthorized(new BaseResponse(401, "Unauthorized", null, []));

    newOrder.AppUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var user = await userManager.FindByIdAsync(newOrder.AppUserId);
    if (user == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

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

  [HttpGet("{id:int}/items")]
  public async Task<IActionResult> GetOrderItems(int id) {
    var order = await orderRepository.GetAsync(x => x.Id == id, "Items");

    if (order == null) {
      return NotFound();
    }

    var response = new BaseResponse(200, "Success", mapper.Map<List<OrderItemGetDto>>(order.OrderItems), []);
    return Ok(response);
  }

  [HttpPost("{id:int}/items")]
  public async Task<IActionResult> CreateOrderItem(int id, OrderItemPostDto orderItem) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var order = await orderRepository.GetAsync(x => x.Id == id);
    if (order == null) {
      return NotFound();
    }

    var newOrderItem = mapper.Map<OrderItem>(orderItem);
    newOrderItem.OrderId = id;

    await orderRepository.AddOrderItemAsync(newOrderItem);
    var response = new BaseResponse(201, "Created", mapper.Map<OrderItemGetDto>(newOrderItem), []);
    return Ok(response);
  }

  [HttpPost("{id:int}/items/{itemId:int}")]
  public async Task<IActionResult> UpdateOrderItem(int id, int itemId, int quantity) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var order = await orderRepository.GetAsync(x => x.Id == id);
    if (order == null) {
      return NotFound();
    }

    var orderItemToUpdate = order.OrderItems.FirstOrDefault(x => x.Id == itemId);
    if (orderItemToUpdate == null) {
      return NotFound();
    }

    orderItemToUpdate.Quantity = quantity;
    await orderRepository.SaveAsync();

    var response = new BaseResponse(204, "Updated", null, []);
    return Ok(response);
  }


  [HttpDelete("{id:int}/items/{itemId:int}")]
  public async Task<IActionResult> DeleteOrderItem(int id, int itemId) {
    var order = await orderRepository.GetAsync(x => x.Id == id);
    if (order == null) {
      return NotFound();
    }

    var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == itemId);
    if (orderItem == null) {
      return NotFound();
    }

    await orderRepository.DeleteOrderItemAsync(orderItem);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }
}