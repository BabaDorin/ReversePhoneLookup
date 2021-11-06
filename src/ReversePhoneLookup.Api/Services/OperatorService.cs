using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Api.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly IPhoneRepository repository;

        public OperatorService(IPhoneRepository repository)
        {
            this.repository = repository;
        }

        public async Task AddOperatorAsync(OperatorViewModelIn @operator, CancellationToken cancellationToken)
        {
            if ((await repository
                .GetOperatorAsync(
                @operator.Mcc,
                @operator.Mnc,
                @operator.Name,
                cancellationToken)) != null)
            {
                throw new ApiException(StatusCode.ValidationError);
            }

            var operatorModel = new Operator()
            {
                Mcc = @operator.Mcc,
                Mnc = @operator.Mnc,
                Name = @operator.Name,
            };

            await repository.AddOperatorAsync(operatorModel, cancellationToken);
        }
    }
}
