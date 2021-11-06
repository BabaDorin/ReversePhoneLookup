using ReversePhoneLookup.Models.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Abstract.Services
{
    public interface IPhoneService
    {
        Task AddPhoneAsync(PhoneViewModelIn phone, CancellationToken cancellationToken);

        string TryFormatPhoneNumber(string phone);
        
        bool IsPhoneNumber(string phone);

        string ValidatePhoneNumber(string phone);
    }
}
