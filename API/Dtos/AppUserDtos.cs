using Core.Entities;

namespace API.Dtos;

public record AppUserGetDto(
  int Id,
  string UserName,
  string Email,
  Gender Gender,
  DateTime Birthday,
  string AvatarUrl
);