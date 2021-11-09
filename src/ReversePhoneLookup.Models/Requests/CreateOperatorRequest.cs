using System.ComponentModel.DataAnnotations;

namespace ReversePhoneLookup.Models.Requests
{
    public class CreateOperatorRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mcc is required")]
        public string Mcc { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mnc is required")]
        public string Mnc { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
