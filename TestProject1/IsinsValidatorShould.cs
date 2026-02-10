using FluentAssertions;
using Securities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject1
{
    public class IsinsValidatorShould
    {
        private IsinsValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new IsinsValidator();
        }

        [Test]
        public async Task Pass_Validation_When_Security_Is_Valid()
        {
            // Arrange
            var request = new ExecuteSecurityRequest
            {
                Isins = new List<string> { "IT1234567890", "IT0987654321" }
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
