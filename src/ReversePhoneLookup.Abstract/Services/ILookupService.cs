using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Services
{
    public interface ILookupService
    {
        Task<LookupResponse> LookupAsync(LookupRequest request, CancellationToken cancellationToken);
    }
}
