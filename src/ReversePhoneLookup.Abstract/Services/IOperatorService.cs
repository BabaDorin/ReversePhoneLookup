using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Services
{
    public interface IOperatorService
    {
        Task<APIResponse> AddOperatorAsync(CreateOperatorRequest request, CancellationToken cancellationToken);
    }
}
