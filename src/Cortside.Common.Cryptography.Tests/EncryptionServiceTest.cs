using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Cortside.Common.Cryptography.Tests {
    public class EncryptionServiceTests {
        private readonly IEncryptionService encryptionService;

        public EncryptionServiceTests() {
            var secret = "*G-KaPdSgVkYp3s6v8y/B?E(H+MbQeTh";
            encryptionService = new EncryptionService(secret);
        }

        [Fact]
        public void ShouldEncryptAndDecryptSearchObject() {
            // arrange
            RebateSearchDto rebateSearchDto = new RebateSearchDto {
                ContractorIds = new List<int> { 1 },
                LoanId = Guid.NewGuid(),
                RebateStatus = RebateRequestStatus.Created
            };

            // act
            string response = encryptionService.EncryptObject(rebateSearchDto);

            // assert
            response.Should().NotBeNullOrWhiteSpace();

            // act
            RebateSearchDto rebateSearchDtoDecrypted = encryptionService.DecryptObject<RebateSearchDto>(response);

            // assert
            rebateSearchDtoDecrypted.Should().NotBeNull();
            rebateSearchDtoDecrypted.ContractorIds.Should().BeEquivalentTo(rebateSearchDto.ContractorIds);
            rebateSearchDtoDecrypted.LoanId.Should().Be(rebateSearchDto.LoanId);
            rebateSearchDtoDecrypted.RebateStatus.Should().Be(rebateSearchDto.RebateStatus);
        }
    }
}
