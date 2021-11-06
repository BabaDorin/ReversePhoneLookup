using Microsoft.Extensions.Caching.Memory;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Models.Services
{
    public class PhoneService : IPhoneService
    {
        private readonly IPhoneRepository repository;

        public PhoneService(IPhoneRepository repository)
        {
            this.repository = repository;
        }
        
        public async Task AddOperatorAsync(OperatorViewModelIn @operator, CancellationToken cancellationToken)
        {
            if((await repository
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

        public async Task AddPhoneAsync(PhoneViewModelIn phone, CancellationToken cancellationToken)
        {
            string formattedPhoneNumber = ValidatePhoneNumber(phone.Value);

            if ((await repository.GetPhoneDataAsync(formattedPhoneNumber, cancellationToken)) != null)
            {
                throw new ApiException(StatusCode.ValidationError);
            }

            var phoneModel = new Phone()
            {
                Value = formattedPhoneNumber,
                OperatorId = phone.OperatorId,
            };

            if (phone.Contacts != null)
            {
                foreach(var contact in phone.Contacts)
                {
                    var contactModel = await GetContactModel(contact, cancellationToken);
                    phoneModel.Contacts.Add(contactModel);
                }
            }

            await repository.AddPhoneAsync(phoneModel, cancellationToken);
        }
        
        public string TryFormatPhoneNumber(string phone)
        {
            try
            {
                phone = phone.Trim();
                if (phone[0] == '0' && phone[1] != '0')
                    return "+373" + phone.Substring(1);
                else if (phone[0] == '0' && phone[1] == '0')
                    return "+373" + phone.Substring(2);
                else if (phone.StartsWith("3730"))
                    return "+373" + phone.Substring(4);
                else if (phone.StartsWith("373"))
                    return "+" + phone;
                else if (phone.StartsWith("+3730"))
                    return "+373" + phone.Substring(5);
                else if (!phone.StartsWith("+373"))
                    return "+373" + phone;
                return phone;
            }
            catch { return phone; }
        }

        public bool IsPhoneNumber(string phone)
        {
            if (!phone.StartsWith("+373"))
                return false;
            string part = phone.Substring(4);
            if (part.Length != 8)
                return false;
            if (!part.All(x => char.IsNumber(x)))
                return false;
            string[] validPrefixes = new string[] { "60", "61", "62", "67", "68", "69", "767", "774", "775", "777", "778", "779", "78", "79" };
            foreach (var prefix in validPrefixes)
            {
                if (part.StartsWith(prefix))
                    return true;
            }
            return false;
        }

        public string ValidatePhoneNumber(string phone)
        {
            var formattedPhoneNumber = TryFormatPhoneNumber(phone);
            
            if (!IsPhoneNumber(formattedPhoneNumber))
                throw new ApiException(StatusCode.InvalidPhoneNumber);

            return formattedPhoneNumber;
        }

        private async Task<Contact> GetContactModel(
            ContactViewModelIn contact, 
            CancellationToken cancellationToken)
        {
            var contactModel = new Contact()
            {
                Name = contact.Name,
            };

            if (contact.Phone != null)
            {
                var formattedNumber = ValidatePhoneNumber(contact.Phone.Value);

                // Find out if the specified phone already exists and assign it.
                // Otherwise - create a new phone model.
                contactModel.Phone = (await repository
                    .GetPhoneDataAsync(formattedNumber, cancellationToken))
                    ??
                    new Phone
                    {
                        Value = formattedNumber,
                        OperatorId = contact.Phone.OperatorId
                    };
            }

            return contactModel;
        }
    }
}
