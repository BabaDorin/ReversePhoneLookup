using System.Collections.Generic;

namespace ReversePhoneLookup.Models.ViewModels
{
    public class PhoneViewModelIn
    {
        public string Value { get; set; }
        public int? OperatorId { get; set; }

        public ICollection<ContactViewModelIn> Contacts { get; set; }
    }
}
