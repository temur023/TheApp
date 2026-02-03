using Clean.Application.Abstractions;
using Clean.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Services;

public class VerifyEmail(IVerifyEmailRepository repository):IVerifyEmailService
{
    public async Task<Status> Handle(Guid tokenId)
    {
        return await repository.Handle(tokenId);
    }
}