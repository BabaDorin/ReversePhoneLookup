using ReversePhoneLookup.Models.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Repositories
{
    public interface IPhoneRepository
    {
        Task<int> AddPhoneAsync(Phone phone, CancellationToken cancellationToken);
        Task UpdatePhoneAsync(Phone phone, CancellationToken cancellationToken);
        Task<Phone> GetPhoneDataAsync(string phone, CancellationToken cancellationToken);
    }
}
