using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;

namespace ReversePhoneLookup.Models.Services
{
    public class LookupService : ILookupService
    {
        private readonly IPhoneService phoneService;
        private readonly IPhoneRepository phoneRepository;
        private readonly IPhoneValidatorService phoneValidator;

        public LookupService(
            IPhoneService phoneService, 
            IPhoneRepository phoneRepository,
            IPhoneValidatorService phoneValidator)
        {
            this.phoneService = phoneService;
            this.phoneRepository = phoneRepository;
            this.phoneValidator = phoneValidator;
        }

        public async Task<LookupResponse> LookupAsync(LookupRequest request, CancellationToken cancellationToken)
        {
            string phone = phoneValidator.TryFormatPhoneNumber(request.Phone);
            if (!phoneValidator.IsPhoneNumber(phone))
                throw new ApiException(StatusCode.InvalidPhoneNumber);

            var data = await phoneRepository.GetPhoneDataAsync(phone, cancellationToken);
            if (data == null)
                throw new ApiException(StatusCode.NoDataFound);

            return new LookupResponse()
            {
                Phone = phone,
                Operator = data.Operator == null ? null : new OperatorResponse()
                {
                    Name = data.Operator.Name,
                    Code = data.Operator.Mcc + data.Operator.Mnc
                },
                Names = data.Contacts?.Select(x => x.Name).ToList()
            };
        }
    }
}
