using Clean.Domain.Entities;

namespace Clean.Application.Abstractions;

public interface IVerifyEmailService
{
    Task<Status> Handle(Guid tokenId);
}