using MediatR;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<User>>; 