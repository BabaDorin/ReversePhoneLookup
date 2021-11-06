using ReversePhoneLookup.Models.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Repositories
{
    public interface IPhoneRepository
    {
        Task<Phone> GetPhoneDataAsync(string phone, CancellationToken cancellationToken);

        Task<int> AddPhoneAsync(Phone phone, CancellationToken cancellationToken);

        Task<int> AddOperatorAsync(Operator @operator, CancellationToken cancellationToken);
    }
}
