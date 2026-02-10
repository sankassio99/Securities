using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Securities;

namespace TestProject1
{
    public class SecurityServiceShould
    {
        private SecurityService _securityService;
        private Mock<IIsinsPricesService> _mockIsinsPricesService;
        private Mock<IPricesRepository> _mockPricesRepository;
        private Mock<IValidator<ExecuteSecurityRequest>> _mockIsinsValidator;

        [SetUp]
        public void Setup()
        {
            _mockIsinsPricesService = new Mock<IIsinsPricesService>();
            _mockPricesRepository = new Mock<IPricesRepository>();
            _mockIsinsValidator = new Mock<IValidator<ExecuteSecurityRequest>>();

            _securityService = new SecurityService(
                _mockIsinsPricesService.Object, _mockPricesRepository.Object, _mockIsinsValidator.Object);
        }

        [Test]
        public async Task Get_Prices_From_IsinsAsync()
        {
            // Arrange
            List<string> isins = new List<string> { "US1234567890", "GB0987654321" };

            // Act
            await _securityService.ExecuteAsync(new ExecuteSecurityRequest() { Isins = isins});

            // Assert
            _mockIsinsPricesService.Verify(mock => mock.GetPrices(isins), Times.Once);
        }

        [Test]
        public async Task Save_Prices()
        {
            // Arrange
            List<string> isins = new List<string> { "US1234567890", "GB0987654321" };

            _mockIsinsPricesService
                .Setup(mock => mock.GetPrices(isins))
                .ReturnsAsync(new List<SecurityPrice>
                {
                    new SecurityPrice("US1234567890", 100.50m),
                    new SecurityPrice("GB0987654321", 200.75m)
                });

            // Act
            await _securityService.ExecuteAsync(new ExecuteSecurityRequest() { Isins = isins });

            // Assert
            var pricesList = new List<PriceEntity>
            {
                new PriceEntity("US1234567890", 100.50m),
                new PriceEntity("GB0987654321", 200.75m)
            };

            _mockPricesRepository.Verify(
                mock => mock.SaveBatch(It.Is<List<PriceEntity>>(list =>
                    list.Count == 2 &&
                    list[0].Isin == "US1234567890" && list[0].Price == 100.50m &&
                    list[1].Isin == "GB0987654321" && list[1].Price == 200.75m
                )),
                Times.Once
            );
        }

        // TODO: test just the fail scenario of validation
        [Test]
        public async Task Should_Validate_Invalid_Isins()
        {
            // Arrange
            List<string> isins = new List<string> { "TI1234567890", "GB0987654321", "IT123" };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Isins[0]", "ISIN must be exactly 12 characters long."),
                new ValidationFailure("Isins[0]", "ISIN must start with 'IT'.")
            });

            _mockIsinsValidator.Setup(v => v.Validate(It.IsAny<ExecuteSecurityRequest>()))
                .Returns(validationResult);

            // Act
            var result = await _securityService.ExecuteAsync(new ExecuteSecurityRequest() { Isins = isins });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
        }
    }
}
