namespace ReversePhoneLookup.Models
{
    public enum StatusCode
    {
        RequestError = 1,
        ServerError = 2,

        InvalidPhoneNumber = 10,
        NoDataFound = 11,
        ValidationError = 12,
        Conflict = 13,

        Created = 20,
        Updated = 21
    }
}
