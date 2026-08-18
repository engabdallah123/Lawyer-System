using Microsoft.AspNetCore.Http;

namespace Shared.Application.IService;

public interface IEmailService
{
    Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? files = null,CancellationToken ct = default);

}