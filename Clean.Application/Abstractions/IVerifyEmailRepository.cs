using Clean.Domain.Entities;

namespace Clean.Application.Abstractions;

public interface IVerifyEmailRepository
{
     Task<Status> Handle(Guid tokenId);
}