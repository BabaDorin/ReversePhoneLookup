using AutoFixture;
using Moq;
using ReversePhoneLookup.Abstract.Services;

namespace ReversePhoneLookup.Api.UnitTests.Shared
{
    public class PhoneValidatorCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Mock<IPhoneValidatorService>>(composer =>
                composer.Do(fake =>
                    fake.Setup(service => service.ValidatePhoneNumber(It.IsAny<string>()))
                        .Returns((string input) => { return input; })));
        }
    }
}
