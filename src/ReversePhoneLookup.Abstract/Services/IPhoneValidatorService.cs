namespace ReversePhoneLookup.Abstract.Services
{
    public interface IPhoneValidatorService
    {
        /// <summary>
        /// Format the passed phone number to international format.
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>The formatted phone number</returns>
        public string TryFormatPhoneNumber(string phone);

        public bool IsPhoneNumber(string phone);

        /// <summary>
        /// Format & validate the passed phone number. Trow an exception if it is not a phone number.
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>The formatted phone number</returns>
        public string ValidatePhoneNumber(string phone);
    }
}