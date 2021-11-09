using System.ComponentModel.DataAnnotations;

namespace ReversePhoneLookup.Models.Requests
{
    public class CreateContactRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
