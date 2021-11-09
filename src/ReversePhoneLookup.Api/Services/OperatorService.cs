using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Api.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository repository;

        public OperatorService(IOperatorRepository repository)
        {
            this.repository = repository;
        }

        public async Task<APIResponse> AddOperatorAsync(
            CreateOperatorRequest request, 
            CancellationToken cancellationToken)
        {
            var existentOperator = await repository
                .GetOperatorAsync(
                    request.Mcc,
                    request.Mnc,
                    request.Name,
                    cancellationToken);

            if (existentOperator != null)
            {
                throw new ApiException(StatusCode.Conflict);
            }

            var operatorModel = new Operator()
            {
                Mcc = request.Mcc,
                Mnc = request.Mnc,
                Name = request.Name,
            };

            await repository.AddOperatorAsync(operatorModel, cancellationToken);

            return new APIResponse(StatusCode.Created, operatorModel.Id);
        }
    }
}
