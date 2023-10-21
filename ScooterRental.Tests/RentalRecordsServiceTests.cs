using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentalRecordsServiceTests
    {
        private List<RentedScooter> _rentedScooterList;
        private RentalRecordsService _rentalRecordsService;
        private const string DEFAULT_SCOOTER_ID = "1";
        private DateTime DEFAULT_RENT_START = DateTime.Now;
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;

        [TestInitialize]
        public void Setup()
        {
            _rentedScooterList = new List<RentedScooter>();
            _rentalRecordsService = new RentalRecordsService(_rentedScooterList);
        }

        [TestMethod]
        public void Constructor_InitializesRentedScooterList()
        {
            _rentalRecordsService.Should().NotBeNull();
        }

        [TestMethod]
        public void GetRentedScooterList_ReturnsRentedScooterList()
        {
            _rentalRecordsService.GetRentedScooterList().Should().BeSameAs(_rentedScooterList);
        }

        [TestMethod]
        public void StartRent_AddsNewRentedScooter()
        {
            _rentalRecordsService.StartRent(DEFAULT_SCOOTER_ID, DEFAULT_RENT_START, DEFAULT_PRICE_PER_MINUTE);

            _rentedScooterList.Count.Should().Be(1);
            var rentedScooter = _rentedScooterList.First();
            rentedScooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            rentedScooter.RentStart.Should().BeCloseTo(DEFAULT_RENT_START, TimeSpan.FromMilliseconds(100));
            rentedScooter.PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);
            rentedScooter.RentEnd.Should().BeNull();
        }

        [TestMethod]
        public void StartRent_AddScooterWithNegativePrice_ThrowsNegativePriceException()
        {
            Action action = () => _rentalRecordsService.StartRent(DEFAULT_SCOOTER_ID, DEFAULT_RENT_START, -0.2m);

            action.Should().Throw<NegativePriceException>();
        }

        [TestMethod]
        public void StartRent_AddScooterWithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _rentalRecordsService.StartRent("", DEFAULT_RENT_START, DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void StopRent_StopsRentalAndSetsRentEnd()
        {
            _rentalRecordsService.StartRent(DEFAULT_SCOOTER_ID, DEFAULT_RENT_START, DEFAULT_PRICE_PER_MINUTE);
            var rentEnd = DateTime.Now.AddMinutes(30);

            var rentedScooter = _rentalRecordsService.StopRent(DEFAULT_SCOOTER_ID, rentEnd);

            rentedScooter.Should().NotBeNull();
            rentedScooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            rentedScooter.RentStart.Should().BeCloseTo(DEFAULT_RENT_START, TimeSpan.FromMilliseconds(100));
            rentedScooter.PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);
            rentedScooter.RentEnd.Should().Be(rentEnd);
        }

        [TestMethod]
        public void StopRent_AddScooterWithEmptyId_ThrowsInvalidIdException()
        {
            var rentEnd = DateTime.Now.AddMinutes(30);
            Action action = () => _rentalRecordsService.StopRent("", rentEnd);

            action.Should().Throw<InvalidIdException>();
        }
    }
}
