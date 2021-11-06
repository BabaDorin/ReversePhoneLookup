using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Services;
using ReversePhoneLookup.Models.UnitTests.Shared;
using ReversePhoneLookup.Models.ViewModels;
using Xunit;

namespace ReversePhoneLookup.Models.UnitTests.Services
{
    public class PhoneServiceTests
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
        public void IsPhoneNumber_ReturnsTrue_OnValidPhone(string phone, PhoneService sut)
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
        public void TryFormatPhoneNumber_ShouldFormatToInternationalFormat(string source, string expected, PhoneService sut)
        {
            var result = sut.TryFormatPhoneNumber(source);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineAutoMoqData("invalid phonenumber")]
        [InlineAutoMoqData("000")]
        public void ValidatePhoneNumber_ShouldThrowApiExceptionIfInvalid(
            string phone,
            PhoneService sut)
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
            PhoneService sut)
        {
            // Act
            var actual = sut.ValidatePhoneNumber(source);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddPhoneAsync_ShouldCallRepositoryAddAsyncOnce(
            [Frozen] Mock<IPhoneRepository> repo,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            PhoneViewModelIn phoneVm = new PhoneViewModelIn()
            {
                Value = "+37369123456",
                OperatorId = 1,
            };

            // Act
            await sut.AddPhoneAsync(phoneVm, cancellationToken);

            // Assert
            repo.Verify(repo => repo.AddPhoneAsync(
                It.Is<Phone>(p => p.Value == phoneVm.Value && p.OperatorId == phoneVm.OperatorId),
                cancellationToken
                ), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddPhoneAsync_ShouldReuseExistingPhoneEntitiesForContacts(
            [Frozen] Mock<IPhoneRepository> repo,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var existingPhone = new Phone()
            {
                Id = 420,
                OperatorId = 1,
                Value = "+37369111111"
            };

            repo.Setup(r => r.GetPhoneDataAsync(existingPhone.Value, cancellationToken))
                .Returns(Task.FromResult(existingPhone));

            PhoneViewModelIn phoneVm = new PhoneViewModelIn()
            {
                Value = "+37369123456",
                OperatorId = 1,
                Contacts = new List<ContactViewModelIn>()
                {
                    new ContactViewModelIn()
                    {
                        Name = "New Contact",
                        Phone = new PhoneViewModelIn()
                        {
                            Value = existingPhone.Value,
                        }
                    }
                }
            };
                
            // Act
            await sut.AddPhoneAsync(phoneVm, cancellationToken);

            // Assert
            repo.Verify(repo => repo.AddPhoneAsync(
                It.Is<Phone>(p => p.Contacts.First().Phone.Id == existingPhone.Id),
                cancellationToken
                ), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddPhoneAsync_AssignContactsWithNewPhonesToPhone(
            [Frozen] Mock<IPhoneRepository> repo,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var newContactPhone = new PhoneViewModelIn()
            {
                OperatorId = 1,
                Value = "+37369123456"
            };

            PhoneViewModelIn phoneVm = new PhoneViewModelIn()
            {
                Value = "+37369123456",
                OperatorId = 1,
                Contacts = new List<ContactViewModelIn>()
                {
                    new ContactViewModelIn()
                    {
                        Name = "New Contact",
                        Phone = newContactPhone
                    }
                }
            };

            // Act
            await sut.AddPhoneAsync(phoneVm, cancellationToken);

            // Assert
            repo.Verify(repo => repo.AddPhoneAsync(
                It.Is<Phone>(
                    p => p.Contacts.First().Phone.OperatorId == newContactPhone.OperatorId
                    && p.Contacts.First().Phone.Value == newContactPhone.Value),
                cancellationToken
                ), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddPhoneAsync_ShouldThrowApiExceptionIfAlreadyExists(
            [Frozen] Mock<IPhoneRepository> repo,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var phoneVm = new PhoneViewModelIn()
            {
                Value = "+37369123456"
            };

            repo.Setup(r => r.GetPhoneDataAsync(phoneVm.Value, cancellationToken))
                .Returns(Task.FromResult(new Phone()));

            // Act
            var exception = await Record.ExceptionAsync(async ()
                => await sut.AddPhoneAsync(phoneVm, cancellationToken));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.ValidationError, (exception as ApiException).Code);
        }
    }
}
