using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Cortside.Common.Security.JsonConverters;
using Newtonsoft.Json;
using Xunit;

namespace Cortside.Common.Security.Tests {
    public class SubjectPrincipalTest {
        private ClaimsPrincipal claimsPrincipal;

        public SubjectPrincipalTest() {
            // arrange
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim("name", "John Doe"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            claimsPrincipal = new ClaimsPrincipal(identity);
        }

        [Fact]
        public void ShouldExposeSubjectClaims() {
            // act
            var subject = SubjectPrincipal.From(claimsPrincipal);

            // assert
            Assert.Equal(3, subject.Claims.Count());
            foreach (var claim in claimsPrincipal.Claims) {
                Assert.Equal(claim.Value, subject.Claims.FirstOrDefault(x => x.Type == claim.Type).Value);
            }
        }

        [Fact]
        public void ShouldSerializeSubjectPrincipal() {
            // act
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JsonClaimConverter());
            settings.Converters.Add(new JsonClaimsPrincipalConverter());
            settings.Converters.Add(new JsonClaimsIdentityConverter());

            var json = JsonConvert.SerializeObject(claimsPrincipal, settings);

            // assert
            Assert.NotNull(json);
        }

        [Fact]
        public void ShouldHandleNoActor() {
            // arrange
            var claims = new List<Claim>() {
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("name", "John Doe"),
                new Claim("given_name", "John"),
                new Claim("family_name", "Doe"),
                new Claim("upn", "john.doe"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            claimsPrincipal = new ClaimsPrincipal(identity);

            // act
            var subject = SubjectPrincipal.From(claimsPrincipal);

            // assert
            Assert.Equal(claims[0].Value, subject.SubjectId);
            Assert.Equal(claims[1].Value, subject.Name);
            Assert.Equal(claims[2].Value, subject.GivenName);
            Assert.Equal(claims[3].Value, subject.FamilyName);
            Assert.Equal(claims[4].Value, subject.UserPrincipalName);
        }

        [Fact]
        public void ShouldHandleActor() {
            // arrange
            var claims = new List<Claim>() {
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("name", "John Doe"),
                new Claim("given_name", "John"),
                new Claim("family_name", "Doe"),
                new Claim("upn", "john.doe"),
                new Claim("act", Guid.NewGuid().ToString()),
                new Claim("act_upn", "actor upn"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            claimsPrincipal = new ClaimsPrincipal(identity);

            // act
            var subject = SubjectPrincipal.From(claimsPrincipal);

            // assert
            Assert.Equal(claims[5].Value, subject.SubjectId);
            Assert.Null(subject.Name);
            Assert.Null(subject.GivenName);
            Assert.Null(subject.FamilyName);
            Assert.Equal(claims[6].Value, subject.UserPrincipalName);
            Assert.NotNull(subject.Actor);
            Assert.Equal(claims[5].Value, subject.Actor.SubjectId);
            Assert.Equal(claims[6].Value, subject.Actor.UserPrincipalName);
            Assert.Null(subject.Actor.Name);
            Assert.Null(subject.Actor.GivenName);
            Assert.Null(subject.Actor.FamilyName);
        }
    }
}
