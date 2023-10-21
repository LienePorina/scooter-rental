using FluentAssertions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class ScooterTests
    {
        private const string DEFAULT_SCOOTER_ID = "1";
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;
        private Scooter scooter;

        [TestInitialize]
        public void Setup()
        {
            scooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void Scooter_Constructor_SetsPropertiesCorrectly()
        {
            scooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            scooter.PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);
            scooter.IsRented.Should().BeFalse();
        }

        [TestMethod]
        public void Scooter_IsRented_SetToTrue()
        {
            scooter.IsRented = true;

            scooter.IsRented.Should().BeTrue();
        }

        [TestMethod]
        public void Scooter_IsRented_SetToFlase()
        {
            scooter.IsRented = false;

            scooter.IsRented.Should().BeFalse();
        }
    }
}