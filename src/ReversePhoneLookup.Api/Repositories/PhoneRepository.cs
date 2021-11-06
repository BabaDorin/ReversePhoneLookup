using Microsoft.EntityFrameworkCore;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Models.Models.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Models.Repositories
{
    public class PhoneRepository : IPhoneRepository
    {
        private readonly PhoneLookupContext context;

        public PhoneRepository(PhoneLookupContext context)
        {
            this.context = context;
        }

        public async Task<int> AddOperatorAsync(Operator @operator, CancellationToken cancellationToken)
        {
            var result = await context.Operators
                .AddAsync(@operator, cancellationToken);

            return result.Entity.Id;
        }

        public async Task<int> AddPhoneAsync(Phone phone, CancellationToken cancellationToken)
        {
            var result = await context.Phones
                .AddAsync(phone, cancellationToken);

            return result.Entity.Id;
        }

        public async Task<Phone> GetPhoneDataAsync(string phone, CancellationToken cancellationToken)
        {
            return await context.Phones
                .Where(x => x.Value == phone)
                .Include(x => x.Operator)
                .Include(x => x.Contacts)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Operator> GetOperatorAsync(int id, CancellationToken cancellationToken)
        {
            return await context.Operators
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Operator> GetOperatorAsync(string mcc, string mnc, string name, CancellationToken cancellationToken)
        {
            return await context.Operators
                .FirstOrDefaultAsync(o => 
                    o.Mcc == mcc 
                    && o.Mnc == mnc
                    && o.Name == name);
        }
    }
}
