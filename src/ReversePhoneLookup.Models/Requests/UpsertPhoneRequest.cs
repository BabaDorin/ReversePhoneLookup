using System.ComponentModel.DataAnnotations;

namespace ReversePhoneLookup.Models.Requests
{
    public class UpsertPhoneRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mnc is required")]
        public string Value { get; set; }
        
        public int OperatorId { get; set; }
        
        public CreateContactRequest Contact { get; set; }
    }
}
