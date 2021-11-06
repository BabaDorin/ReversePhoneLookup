using AutoFixture.Xunit2;
using Moq;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Api.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.UnitTests.Shared;
using ReversePhoneLookup.Models.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.Api.UnitTests.Services
{
    public class OperatorServiceTests
    {
        [Theory]
        [AutoMoqData]
        public async Task AddOperatorAsync_ShouldCallRepositoryAddAsyncOnce(
            [Frozen] Mock<IPhoneRepository> repo,
            OperatorService sut,
            OperatorViewModelIn @operator,
            CancellationToken cancellationToken)
        {
            // Act
            await sut.AddOperatorAsync(@operator, cancellationToken);

            // Assert
            repo.Verify(r => r.AddOperatorAsync(
                It.Is<Operator>(
                    op => op.Mnc == @operator.Mnc
                    && op.Mcc == @operator.Mcc
                    && op.Name == @operator.Name),
                cancellationToken
                ), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOperatorAsync_ShouldThrowExceptionIfAlreadyExists(
            [Frozen] Mock<IPhoneRepository> repo,
            OperatorService sut,
            OperatorViewModelIn @operator,
            CancellationToken cancellationToken)
        {
            // Arrange
            repo.Setup(r => r.GetOperatorAsync(
                    @operator.Mcc,
                    @operator.Mnc,
                    @operator.Name,
                    cancellationToken))
                .Returns(Task.FromResult(new Operator()));

            // Act
            var exception = await Record
                .ExceptionAsync(async () => await sut.AddOperatorAsync(@operator, cancellationToken));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.ValidationError, (exception as ApiException).Code);
        }
    }
}
