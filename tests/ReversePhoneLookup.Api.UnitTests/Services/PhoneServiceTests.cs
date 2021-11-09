using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Services;
using ReversePhoneLookup.Models.UnitTests.Shared;
using Xunit;

namespace ReversePhoneLookup.Models.UnitTests.Services
{
    public class PhoneServiceTests
    {
        [Theory]
        [AutoMoqData(typeof(Behaviours), nameof(Behaviours.GenerationDepthBehaviorDepth2))]
        public async Task AddOrUpdatePhoneAsync_ShouldCallValidatePhoneNumber(
            [Frozen] Mock<IPhoneValidatorService> phoneValidatorMock,
            PhoneService sut,
            UpsertPhoneRequest request,
            CancellationToken cancellationToken)
        {
            // Act
            await Record.ExceptionAsync(() => sut.AddOrUpdatePhoneAsync(request, cancellationToken));

            // Assert
            phoneValidatorMock.Verify(m => m.ValidatePhoneNumber(request.Value));
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOrUpdatePhoneAsync_NewPhone_ShouldAddPhone(
            [Frozen] Mock<IPhoneRepository> phoneRepositoryMock,
            [Frozen] Mock<IOperatorRepository> operatorRepositoryMock,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var request = new UpsertPhoneRequest()
            {
                Value = "+37369123456",
                OperatorId = 1,
                Contact = new CreateContactRequest() { Name = "New Contact" }
            };

            operatorRepositoryMock.Setup(r => r.GetOperatorAsync(request.OperatorId, cancellationToken))
                 .ReturnsAsync(new Operator());

            // Act
            await sut.AddOrUpdatePhoneAsync(request, cancellationToken);

            // Assert
            phoneRepositoryMock.Verify(repo => repo
                .AddPhoneAsync(
                    It.Is<Phone>(p => 
                        p.Value == request.Value 
                        && p.OperatorId == request.OperatorId
                        && p.Contacts.First().Name == request.Contact.Name),
                    cancellationToken),
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOrUpdatePhoneAsync_ExistentPhone_NewContact_ShouldAddContactToPhone(
            [Frozen] Mock<IPhoneRepository> phoneRepositoryMock,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var existentPhone = new Phone()
            {
                Id = 420,
                OperatorId = 1,
                Value = "+37369123456"
            };

            var request = new UpsertPhoneRequest()
            {
                Value = existentPhone.Value,
                OperatorId = (int)existentPhone.OperatorId,
                Contact = new CreateContactRequest { Name = "New Contact" }
            };

            phoneRepositoryMock.Setup(r => r.GetPhoneDataAsync(request.Value, cancellationToken))
                .ReturnsAsync(existentPhone);
            
            // Act
            await sut.AddOrUpdatePhoneAsync(request, cancellationToken);

            // Assert
            phoneRepositoryMock.Verify(repo => repo
                .UpdatePhoneAsync(
                    It.Is<Phone>(p => 
                        p.Value == request.Value
                        && p.Contacts.First().Name == request.Contact.Name),
                    cancellationToken), 
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOrUpdatePhoneAsync_ExistentPhone_ExistentContact_ShouldThrowApiException(
            [Frozen] Mock<IPhoneRepository> phoneRepositoryMock,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var existentContact = new Contact
            {
                Name = "Existent contact"
            };

            var existentPhone = new Phone()
            {
                Value = "+37369123456",
                OperatorId = 1,
                Contacts = new List<Contact>() { existentContact }
            };

            var request = new UpsertPhoneRequest
            {
                Value = existentPhone.Value,
                OperatorId = (int)existentPhone.OperatorId,
                Contact = new CreateContactRequest { Name = existentContact.Name }
            };

            phoneRepositoryMock.Setup(r => r.GetPhoneDataAsync(request.Value, cancellationToken))
                .Returns(Task.FromResult(existentPhone));

            // Act
            var exception = await Record.ExceptionAsync(() => 
                sut.AddOrUpdatePhoneAsync(request, cancellationToken));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.Conflict, (exception as ApiException).Code);

            phoneRepositoryMock.Verify(r => r
                .AddPhoneAsync(
                    It.IsAny<Phone>(),
                    cancellationToken),
                Times.Never);

            phoneRepositoryMock.Verify(r => r
                .UpdatePhoneAsync(
                    It.IsAny<Phone>(),
                    cancellationToken),
                Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOrUpdatePhoneAsync_InvalidOperatorId_ShouldThrowApiException(
            [Frozen] Mock<IOperatorRepository> operatorRepositoryMock,
            PhoneService sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            var request = new UpsertPhoneRequest
            {
                Value = "+37369123456",
                OperatorId = 1, // does not exist
            };

            operatorRepositoryMock.Setup(r => r.GetOperatorAsync(request.OperatorId, cancellationToken))
                 .ReturnsAsync((Operator)null);

            // Act
            var exception = await Record.ExceptionAsync(() => 
                sut.AddOrUpdatePhoneAsync(request, cancellationToken));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.NoDataFound, (exception as ApiException).Code);
        }
    }
}
