using AutoFixture.Xunit2;
using Moq;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Api.Services;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.UnitTests.Shared;
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
            [Frozen] Mock<IOperatorRepository> repo,
            OperatorService sut,
            CreateOperatorRequest request,
            CancellationToken cancellationToken)
        {
            // Act
            await sut.AddOperatorAsync(request, cancellationToken);

            // Assert
            repo.Verify(r => r
                .AddOperatorAsync(
                    It.Is<Operator>(
                        op => op.Mnc == request.Mnc
                        && op.Mcc == request.Mcc
                        && op.Name == request.Name),
                    cancellationToken), 
                Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task AddOperatorAsync_ShouldThrowApiExceptionIfAlreadyExists(
            [Frozen] Mock<IOperatorRepository> repo,
            OperatorService sut,
            CreateOperatorRequest request,
            CancellationToken cancellationToken)
        {
            // Arrange
            repo.Setup(r => r.GetOperatorAsync(
                    request.Mcc,
                    request.Mnc,
                    request.Name,
                    cancellationToken))
                .Returns(Task.FromResult(new Operator()));

            // Act
            var exception = await Record
                .ExceptionAsync(async () => await sut.AddOperatorAsync(request, cancellationToken));

            // Assert
            Assert.True(exception is ApiException);
            Assert.Equal(StatusCode.Conflict, (exception as ApiException).Code);
        }
    }
}
