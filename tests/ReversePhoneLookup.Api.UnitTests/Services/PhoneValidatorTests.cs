using ReversePhoneLookup.Api.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.UnitTests.Shared;
using Xunit;

namespace ReversePhoneLookup.Api.UnitTests.Services
{
    public class PhoneValidatorTests
    {
        [Theory]
        [InlineAutoMoqData("+37360123456")]
        [InlineAutoMoqData("+37361123456")]
        [InlineAutoMoqData("+37362123456")]
        [InlineAutoMoqData("+37367123456")]
        [InlineAutoMoqData("+37368123456")]
        [InlineAutoMoqData("+37369123456")]
        [InlineAutoMoqData("+37376712345")]
        [InlineAutoMoqData("+37377412345")]
        [InlineAutoMoqData("+37377512345")]
        [InlineAutoMoqData("+37377712345")]
        [InlineAutoMoqData("+37377812345")]
        [InlineAutoMoqData("+37377912345")]
        [InlineAutoMoqData("+37378123456")]
        [InlineAutoMoqData("+37379123456")]
        public void IsPhoneNumber_ReturnsTrue_OnValidPhone(
            string phone, 
            PhoneValidatorService sut)
        {
            var result = sut.IsPhoneNumber(phone);

            Assert.True(result);
        }

        [Theory]
        [InlineAutoMoqData("69123456", "+37369123456")]
        [InlineAutoMoqData("069123456", "+37369123456")]
        [InlineAutoMoqData("0069123456", "+37369123456")]
        [InlineAutoMoqData("37369123456", "+37369123456")]
        [InlineAutoMoqData("373069123456", "+37369123456")]
        [InlineAutoMoqData("+37369123456", "+37369123456")]
        [InlineAutoMoqData("+373069123456", "+37369123456")]
        public void TryFormatPhoneNumber_ShouldFormatToInternationalFormat(
            string source, 
            string expected, 
            PhoneValidatorService sut)
        {
            var result = sut.TryFormatPhoneNumber(source);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineAutoMoqData("invalid phonenumber")]
        [InlineAutoMoqData("000")]
        public void ValidatePhoneNumber_ShouldThrowApiExceptionIfInvalid(
            string phone,
            PhoneValidatorService sut)
        {
            // Act
            var exception = Record.Exception(() => sut.ValidatePhoneNumber(phone));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.InvalidPhoneNumber, (exception as ApiException).Code);
        }

        [Theory]
        [InlineAutoMoqData("69123456", "+37369123456")]
        [InlineAutoMoqData("069123456", "+37369123456")]
        public void ValidatePhoneNumber_ShouldFormatToInternationFormat(
            string source,
            string expected,
            PhoneValidatorService sut)
        {
            // Act
            var actual = sut.ValidatePhoneNumber(source);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
