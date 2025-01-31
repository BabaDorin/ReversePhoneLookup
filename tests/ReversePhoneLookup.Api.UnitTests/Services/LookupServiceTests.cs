﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Services;
using ReversePhoneLookup.Models.UnitTests.Shared;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Requests;
using Xunit;

namespace ReversePhoneLookup.Models.UnitTests.Services
{
    public class LookupServiceTests
    {
        [Theory]
        [AutoMoqData]
        public async Task LookupAsync_TryFormatPhoneNumber_CalledOnce(
            [Frozen] Mock<IPhoneValidatorService> phoneValidatorMock,
            LookupRequest request,
            LookupService sut)
        {
            await Record.ExceptionAsync(() => sut.LookupAsync(request, CancellationToken.None));

            phoneValidatorMock.Verify(x => x.TryFormatPhoneNumber(request.Phone), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task LookupAsync_InvalidPhone_ShouldThrowException(
            [Frozen] Mock<IPhoneValidatorService> phoneServiceMock,
            LookupRequest request,
            LookupService sut)
        {
            phoneServiceMock.Setup(x => x.IsPhoneNumber(It.IsAny<string>())).Returns(false);

            await AssertExtensions.ThrowsWithCodeAsync(() => sut.LookupAsync(request, CancellationToken.None), StatusCode.InvalidPhoneNumber);
        }

        [Theory]
        [AutoMoqData]
        public async Task LookupAsync_NoPhoneData_ShouldThrowException(
            [Frozen] Mock<IPhoneService> phoneServiceMock,
            [Frozen] Mock<IPhoneValidatorService> phoneValidatorMock,
            [Frozen] Mock<IPhoneRepository> phoneRepositoryMock,
            LookupRequest request,
            LookupService sut)
        {
            phoneValidatorMock.Setup(x => x.IsPhoneNumber(It.IsAny<string>())).Returns(true);
            phoneRepositoryMock.Setup(x => x.GetPhoneDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Phone)null);

            await AssertExtensions.ThrowsWithCodeAsync(() => sut.LookupAsync(request, CancellationToken.None), StatusCode.NoDataFound);
        }

        [Theory]
        [AutoMoqData(typeof(Behaviours), nameof(Behaviours.GenerationDepthBehaviorDepth2))]
        public async Task LookupAsync_DataFound_ShouldReturnValidResponse(
            [Frozen] Mock<IPhoneValidatorService> phoneValidatorMock,
            [Frozen] Mock<IPhoneRepository> phoneRepositoryMock,
            string formattedPhone,
            Phone phone,
            LookupRequest request,
            LookupService sut)
        {
            phoneValidatorMock.Setup(x => x.TryFormatPhoneNumber(It.IsAny<string>())).Returns(formattedPhone);
            phoneValidatorMock.Setup(x => x.IsPhoneNumber(It.IsAny<string>())).Returns(true);
            phoneRepositoryMock.Setup(x => x.GetPhoneDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(phone);

            var result = await sut.LookupAsync(request, CancellationToken.None);

            Assert.Equal(formattedPhone, result.Phone);
            Assert.Equal(phone.Operator.Name, result.Operator.Name);
            Assert.Equal(phone.Operator.Mcc + phone.Operator.Mnc, result.Operator.Code);
        }
    }
}
