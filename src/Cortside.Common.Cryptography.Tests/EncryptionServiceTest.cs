using System;
using Newtonsoft.Json;
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
                ContractorIds = [1],
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

        [Fact]
        public void DecryptEmptyObject() {
            var response = encryptionService.EncryptString("{}");
            var result = encryptionService.DecryptObject<RebateSearchDto>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void DecryptEmptyJsonToReferenceType() {
            var response = encryptionService.EncryptString("");
            Assert.Throws<JsonSerializationException>(() => encryptionService.DecryptObject<RebateSearchDto>(response));
        }
    }
}
