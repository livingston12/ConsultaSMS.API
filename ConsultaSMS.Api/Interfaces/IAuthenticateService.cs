using System.Threading.Tasks;
using WebConsultaSMS.Models;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Interfaces
{
    public interface IAuthenticateService
    {
        Task<AuthenticateResponse> PostAsync(AuthenticateRequest request);
        Task<AuthenticateResponse> PostInternalAsync(AuthenticateInternalRequest request);
        Task<(string message, bool next)> ValidateUserExist(AuthenticateRequest requestAuth);
    }
}
