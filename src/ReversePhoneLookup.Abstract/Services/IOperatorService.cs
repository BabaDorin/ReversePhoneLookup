using ReversePhoneLookup.Models.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Services
{
    public interface IOperatorService
    {
        Task AddOperatorAsync(OperatorViewModelIn @operator, CancellationToken cancellationToken);
    }
}
