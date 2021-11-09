using Microsoft.EntityFrameworkCore;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Api.Repositories
{
    public class OperatorRepository : IOperatorRepository
    {
        private readonly PhoneLookupContext context;

        public OperatorRepository(PhoneLookupContext context)
        {
            this.context = context;
        }

        public async Task<int> AddOperatorAsync(Operator @operator, CancellationToken cancellationToken)
        {
            var result = await context.Operators.AddAsync(@operator, cancellationToken);
            await context.SaveChangesAsync();
           
            return result.Entity.Id;
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
