using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Models.Services
{
    public class PhoneService : IPhoneService
    {
        private readonly IPhoneRepository repository;
        private readonly IPhoneValidatorService phoneValidator;
        private readonly IOperatorRepository operatorRepository;

        public PhoneService(
            IPhoneRepository repository,
            IPhoneValidatorService phoneValidator,
            IOperatorRepository operatorRepository)
        {
            this.repository = repository;
            this.phoneValidator = phoneValidator;
            this.operatorRepository = operatorRepository;
        }

        public async Task<APIResponse> AddOrUpdatePhoneAsync(
            UpsertPhoneRequest request, 
            CancellationToken cancellationToken)
        {
            // If phone number is new => returns Created + new phone Id
            // If phone number exists, but contact is new => returns Updated 
            // If both phone number and associated contact exists => returns Conflict

            request.Value = phoneValidator.ValidatePhoneNumber(request.Value);

            var existentPhone = await repository.GetPhoneDataAsync(request.Value, cancellationToken);
            if (existentPhone != null)
            {
                var newContact = new Contact { Name = request.Contact.Name };
                
                return await AddContactIfNewAsync(existentPhone, newContact, cancellationToken);
            }

            return await CreatePhoneAsync(request, cancellationToken);
        }

        private async Task<APIResponse> AddContactIfNewAsync(
            Phone phone, 
            Contact contact,
            CancellationToken cancellationToken)
        {
            var contactExists = phone.Contacts
                    .Any(c => c.Name.ToUpper() == contact.Name.ToUpper());

            if (!contactExists)
            {
                phone.Contacts.Add(contact);
                await repository.UpdatePhoneAsync(phone, cancellationToken);

                return new APIResponse(StatusCode.Updated);
            }

            throw new ApiException(StatusCode.Conflict);
        }

        private async Task<APIResponse> CreatePhoneAsync(
            UpsertPhoneRequest request, 
            CancellationToken cancellationToken)
        {
            var phoneOperator = await operatorRepository.GetOperatorAsync(
                request.OperatorId,
                cancellationToken);

            if(phoneOperator is null)
            {
                throw new ApiException(StatusCode.NoDataFound);
            }

            var phone = new Phone()
            {
                Value = request.Value,
                OperatorId = request.OperatorId,
            };

            if(request.Contact != null)
            {
                var newContact = new Contact { Name = request.Contact.Name };
                phone.Contacts.Add(newContact);
            }

            await repository.AddPhoneAsync(phone, cancellationToken);

            return new APIResponse(StatusCode.Created, phone.Id);
        }
    }
}