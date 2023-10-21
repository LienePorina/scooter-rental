using FluentAssertions;
using Moq;
using Moq.AutoMock;
using ScooterRental;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRentalMoq.Tests
{
    [TestClass]
    public class RentalCompanyTests
    {
        private AutoMocker _mocker;
        private IRentalCompany _company;
        private string _companyName;
        private Mock<IRentalRecordsService> _recordsServiceMock;
        private Mock<IPriceCalculation> _priceCalculation;

        private const string DEFAULT_ID = "1";

        [TestInitialize]
        public void SetUp()
        {
            _companyName = "default company";
            _mocker = new AutoMocker();
            var scooterServiceMock = _mocker.GetMock<IScooterService>();
            var recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();
            var priceCalculation = _mocker.GetMock<IPriceCalculation>();
            _company = new RentalCompany(_companyName, scooterServiceMock.Object, recordsServiceMock.Object, priceCalculation.Object);
        }

        [TestMethod]
        public void Constructor_InitializesCompanyName()
        {
            _company.Name.Should().Be(_companyName);
        }

        [TestMethod]
        public void StartRent_WithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _company.StartRent("");

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void StartRent_WithIdNotExists_ThrowscooterNotFound()
        {
            Action action = () => _company.StartRent(DEFAULT_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void StartRent_ScooterRentStarted()
        {
            var scooter = new Scooter(DEFAULT_ID, 0.1m);
            _mocker.GetMock<IScooterService>().Setup(s => s.GetScooterById(DEFAULT_ID)).Returns(scooter);

            _company.StartRent(DEFAULT_ID);

            scooter.IsRented.Should().BeTrue();
            _mocker.GetMock<IRentalRecordsService>().Verify(r => r.StartRent(DEFAULT_ID, It.IsAny<DateTime>(), 0.1m), Times.Once);
        }

        [TestMethod]
        public void EndRent_ValidScooterIdRentedFor10Minutes_ReturnsEndRent10()
        {
            DateTime rentStart = new DateTime(2020, 1, 1, 0, 0, 0, 0);

            var scooter = new Scooter(DEFAULT_ID, 1m) { IsRented = true };
            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, 1m) { RentEnd = rentStart.AddMinutes(10) };
            var rentedScooters = new List<RentedScooter> { rentedScooter };

            var scooterserviceMock = _mocker.GetMock<IScooterService>();
            scooterserviceMock.Setup(s => s.GetScooterById(DEFAULT_ID)).Returns(scooter);

            var recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();
            recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            PriceCalculation priceCalculation = new PriceCalculation(recordsServiceMock.Object);

            RentalCompany company = new RentalCompany(_companyName, scooterserviceMock.Object, recordsServiceMock.Object, priceCalculation);

            decimal result = company.EndRent(DEFAULT_ID);

            result.Should().Be(10m);
            scooter.IsRented.Should().BeFalse();
        }

        [TestMethod]
        public void EndRent_WithNotRentedScooter_ThrowsScooterNotFoundException()
        {
            var rentedScooters = new List<RentedScooter>
            {
                    new RentedScooter("2", DateTime.Now, 0.2m)
                    {
                        rentEnd = DateTime.Now.AddMinutes(30)
                    }
            };

            Action action = () => _company.EndRent("1");

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void CalculateIncome_ReturnsResultFromCalculateTotalIncome()
        {
            int? year = 2023;
            bool includeNotCompletedRentals = false;
            decimal expectedIncome = 42.0m;

            var priceCalculation = new Mock<IPriceCalculation>();
            priceCalculation.Setup(pc => pc.CalculateTotalIncome(year, includeNotCompletedRentals)).Returns(expectedIncome);

            var scooterServiceMock = _mocker.GetMock<IScooterService>();
            var recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();

            RentalCompany company = new RentalCompany(_companyName, scooterServiceMock.Object, recordsServiceMock.Object, priceCalculation.Object);

            decimal actualIncome = company.CalculateIncome(year, includeNotCompletedRentals);

            actualIncome.Should().Be(expectedIncome);
        }
    }
}