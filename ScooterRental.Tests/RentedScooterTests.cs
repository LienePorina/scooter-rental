using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentedScooterTests
    {
        private const string DEFAULT_ID = "1";
        private DateTime DEFAULT_START_TIME = DateTime.Now;
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;

        [TestMethod]
        public void Constructor_WithValidArguments_ShouldInitializeRentedScooter()
        {
            RentedScooter _rentedScooter = new RentedScooter(DEFAULT_ID, DEFAULT_START_TIME, DEFAULT_PRICE_PER_MINUTE);

            _rentedScooter.Id.Should().Be(DEFAULT_ID);
            _rentedScooter.RentStart.Should().Be(DEFAULT_START_TIME);
            _rentedScooter.PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);

            _rentedScooter.RentEnd.Should().BeNull();
        }

        [TestMethod]
        public void Constructor_WithNegativePrice_ThrowsNegativePriceException()
        {
            Action action = () => new RentedScooter(DEFAULT_ID, DEFAULT_START_TIME, -0.2m);

            action.Should().Throw<NegativePriceException>();
        }

        [TestMethod]
        public void SetRentEnd_DateValue_ShouldSetRentEnd()
        {
            RentedScooter _rentedScooter = new RentedScooter(DEFAULT_ID, DEFAULT_START_TIME, DEFAULT_PRICE_PER_MINUTE);

            DateTime rentEnd = DEFAULT_START_TIME.AddMinutes(30);
            _rentedScooter.RentEnd = rentEnd;

            _rentedScooter.RentEnd.Should().Be(rentEnd);
        }

        [TestMethod]
        public void SetRentEnd_NullValue_ShouldSetRentEndToNull()
        {
            RentedScooter _rentedScooter = new RentedScooter("2", DEFAULT_START_TIME, DEFAULT_PRICE_PER_MINUTE);

            _rentedScooter.RentEnd = null;

            _rentedScooter.RentEnd.Should().BeNull();
        }

        [TestMethod]
        public void SetRentEndBeforeRentStart_ThrowsInvalidRentEndException()
        {
            var scooter = new RentedScooter(DEFAULT_ID, DEFAULT_START_TIME, DEFAULT_PRICE_PER_MINUTE);

            Action action = () => scooter.RentEnd = DEFAULT_START_TIME.AddMinutes(-30);

            action.Should().Throw<InvalidRentEndException>();
        }
    }
}