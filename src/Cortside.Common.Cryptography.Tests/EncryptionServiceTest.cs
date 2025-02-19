using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Cortside.Common.Cryptography.Tests {
    public class EncryptionServiceTests {
        private readonly IEncryptionService encryptionService;

        public EncryptionServiceTests() {
            const string secret = "secret";
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
            response.ShouldNotBeNullOrWhiteSpace();

            // act
            RebateSearchDto rebateSearchDtoDecrypted = encryptionService.DecryptObject<RebateSearchDto>(response);

            // assert
            rebateSearchDtoDecrypted.ShouldNotBeNull();
            rebateSearchDtoDecrypted.ContractorIds.ShouldBeEquivalentTo(rebateSearchDto.ContractorIds);
            rebateSearchDtoDecrypted.LoanId.ShouldBe(rebateSearchDto.LoanId);
            rebateSearchDtoDecrypted.RebateStatus.ShouldBe(rebateSearchDto.RebateStatus);
        }
    }
}
