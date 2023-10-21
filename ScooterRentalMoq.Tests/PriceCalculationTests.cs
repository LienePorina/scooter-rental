using FluentAssertions;
using Moq;
using Moq.AutoMock;
using ScooterRental;
using ScooterRental.Exceptions;

namespace ScooterRentalMoq.Tests
{
    [TestClass]
    public class PriceCalculationTests
    {
        private AutoMocker _mocker;
        private Mock<IRentalRecordsService> _recordsServiceMock;
        private PriceCalculation _priceCalculation;
        private string _companyName;
        private const string DEFAULT_ID = "1";

        [TestInitialize]
        public void Setup()
        {
            _mocker = new AutoMocker();
            _recordsServiceMock = new Mock<IRentalRecordsService>();
            _priceCalculation = new PriceCalculation(_recordsServiceMock.Object);
        }

        [TestMethod]
        public void Constructor_RentalRecordsService_SetUpCorrectly()
        {
            var rentalRecordsServiceMock = new Mock<IRentalRecordsService>();

            var priceCalculation = new PriceCalculation(rentalRecordsServiceMock.Object);

            priceCalculation.Should().NotBeNull();
            priceCalculation.Should().BeOfType<PriceCalculation>();
        }

        [TestMethod]
        public void CalculateRentDetailsForCompletedRent_NullId_ThrowsInvalidIdException()
        {
            string id = null;
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);

            DateTime rentEnd = rentStart.AddDays(2).AddHours(0).AddMinutes(50);
            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(id, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            Action action = () => _priceCalculation.CalculateRentDetailsForOneCompletedRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void CalculateRentDetailsForCompletedRent_EmptyId_ThrowsInvalidIdException()
        {
            string id = "";
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);

            DateTime rentEnd = rentStart.AddDays(2).AddHours(0).AddMinutes(50);
            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(id, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            Action action = () => _priceCalculation.CalculateRentDetailsForOneCompletedRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void CalculateRentDetailsForCompletedRent_ValidId_RentDetailsSet()
        {
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);

            DateTime rentEnd = rentStart.AddDays(2).AddHours(0).AddMinutes(50);
            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            var result = _priceCalculation.CalculateRentDetailsForOneCompletedRent(DEFAULT_ID);
            result.firstDayMinutes.Should().Be(30);
            result.daysBetween.Should().Be(2);
            result.lastDayMinutes.Should().Be(20);
        }

        [TestMethod]
        public void CalculateRentDetailsForCompletedRent_ValidIdAndScooter_RentDetailsSetTo0()
        {
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);

            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = null
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            var result = _priceCalculation.CalculateRentDetailsForOneCompletedRent(DEFAULT_ID);

            result.firstDayMinutes.Should().Be(0);
            result.daysBetween.Should().Be(0);
            result.lastDayMinutes.Should().Be(0);
        }

        [TestMethod]
        public void CalculateRentDetailsForCompletedRent_ScooterNotRented_RentDetailsNotSet()
        {
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter>());

            Action action = () => _priceCalculation.CalculateRentDetailsForOneCompletedRent(DEFAULT_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void CalculateRentDetailsForNotCompletedRent_ValidId_RentDetailsSet()
        {
            DateTime rentStart = new DateTime(2023, 9, 10, 1, 30, 0, 0);
            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = null
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            var result = _priceCalculation.CalculateRentDetailsForOneNotCompletedRent(DEFAULT_ID);

            result.firstDayMinutes.Should().BeGreaterOrEqualTo(0);
            result.daysBetween.Should().BeGreaterOrEqualTo(0);
            result.lastDayMinutes.Should().BeGreaterOrEqualTo(0);
        }

        [TestMethod]
        public void CalculateRentDetailsForNotCompletedRent_ValidIdAndScooterRentAlreadyEnded_RentDetailsSetTo0()
        {
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);
            DateTime rentEnd = new DateTime(2023, 9, 3, 1, 0, 0);

            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            var result = _priceCalculation.CalculateRentDetailsForOneNotCompletedRent(DEFAULT_ID);

            result.firstDayMinutes.Should().Be(0);
            result.daysBetween.Should().Be(0);
            result.lastDayMinutes.Should().Be(0);
        }

        [TestMethod]
        public void CalculateRentDetailsForNotCompletedRent_ScooterNotRented_RentDetailsNotSet()
        {
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter>());

            Action action = () => _priceCalculation.CalculateRentDetailsForOneNotCompletedRent(DEFAULT_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void GetRentedScooter_WithValidIdAndRentedScooter_GetScooter()
        {
            DateTime rentStart = new DateTime(2023, 9, 2, 23, 30, 0, 0);
            DateTime rentEnd = new DateTime(2023, 9, 3, 1, 0, 0);

            decimal pricePerMinute = 1m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter });

            RentedScooter scooter = _priceCalculation.GetRentedScooter(DEFAULT_ID);
            scooter.Should().NotBeNull();
            scooter.PricePerMinute.Should().Be(pricePerMinute);
            scooter.RentStart.Should().Be(rentStart);
            scooter.rentEnd.Should().Be(rentEnd);
        }

        [TestMethod]
        public void GetRentedScooter_WithValidIdAndNotRentedScooter_ThrowsScooterNotFoundException()
        {
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter>());

            Action action = () => _priceCalculation.GetRentedScooter(DEFAULT_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_ValidRentalForMultipleDays()
        {
            DateTime rentStart = DateTime.Now;
            DateTime rentEnd = rentStart.AddMinutes(60 * 24 * 2 + 30);
            decimal pricePerMinute = 0.01m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };
            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            decimal expectedIncome = 30 * pricePerMinute + 60 * 24 * 2 * pricePerMinute;
            decimal tolerance = 0.01m;

            result.Should().BeApproximately(expectedIncome, tolerance);
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_ValidRentalForMultipleDaysWithHighPricePerMinute()
        {
            DateTime rentStart = new DateTime(2023, 9, 6, 0, 0, 0, 0);
            DateTime rentEnd = rentStart.AddDays(2).AddMinutes(30);
            decimal pricePerMinute = 0.03m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };

            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            decimal expectedIncome = (30 * pricePerMinute) + (2 * 20);
            decimal tolerance = 0.01m;

            result.Should().BeApproximately(expectedIncome, tolerance);
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_ValidRentalForMultipleDaysWithAlmoustFoullDay()
        {
            DateTime rentStart = new DateTime(2023, 9, 6, 0, 0, 0, 0);
            DateTime rentEnd = rentStart.AddDays(2).AddHours(23);
            decimal pricePerMinute = 0.03m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };
            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            decimal expectedIncome = 20 + 2 * 20;
            decimal tolerance = 0.01m;

            result.Should().BeApproximately(expectedIncome, tolerance);
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_ValidRentalForOneDay()
        {
            DateTime rentStart = new DateTime(2023, 9, 6, 0, 0, 0, 0);
            DateTime rentEnd = rentStart.AddHours(6);
            decimal pricePerMinute = 0.03m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };
            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            decimal expectedIncome = 6 * 60 * pricePerMinute;
            decimal tolerance = 0.01m;

            result.Should().BeApproximately(expectedIncome, tolerance);
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_ValidRentalForOneAlmoustFullDay()
        {
            DateTime rentStart = new DateTime(2023, 9, 6, 0, 0, 0, 0);
            DateTime rentEnd = rentStart.AddHours(23);
            decimal pricePerMinute = 0.03m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = rentEnd
            };
            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            decimal expectedIncome = 20;
            decimal tolerance = 0.01m;

            result.Should().BeApproximately(expectedIncome, tolerance);
        }

        [TestMethod]
        public void CalculateIncomeFromOneRent_NotCompletedRental()
        {
            DateTime rentStart = new DateTime(2023, 9, 9, 0, 0, 0, 0);
            decimal pricePerMinute = 0.03m;

            var rentedScooter = new RentedScooter(DEFAULT_ID, rentStart, pricePerMinute)
            {
                RentEnd = null
            };
            var rentedScooters = new List<RentedScooter> { rentedScooter };
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(rentedScooters);

            var result = _priceCalculation.CalculateIncomeFromOneRent(DEFAULT_ID);

            result.Should().BePositive();
        }

        [TestMethod]
        public void CalculateTotalIncome_ForAllPeriodIncludedNotFinishedRents_CalculateIncome()
        {
            string id1 = "1";
            DateTime rentStart1 = new DateTime(2020, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd1 = rentStart1.AddDays(2);
            decimal pricePerMinute1 = 1m;

            string id2 = "2";
            DateTime rentStart2 = new DateTime(2021, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd2 = rentStart2.AddDays(2);
            decimal pricePerMinute2 = 1m;

            string id3 = "3";
            DateTime rentStart3 = new DateTime(2023, 9, 10, 2, 0, 0, 0);
            decimal pricePerMinute3 = 1m;

            var rentedScooter1 = new RentedScooter(id1, rentStart1, pricePerMinute1)
            {
                RentEnd = rentEnd1
            };
            var scooter1 = new Scooter(id1, pricePerMinute1);


            var rentedScooter2 = new RentedScooter(id2, rentStart2, pricePerMinute2)
            {
                RentEnd = rentEnd2
            };
            var scooter2 = new Scooter(id2, pricePerMinute2);

            var rentedScooter3 = new RentedScooter(id3, rentStart3, pricePerMinute3)
            {
                RentEnd = null
            };
            var scooter3 = new Scooter(id3, pricePerMinute3);

            var _scooterserviceMock = _mocker.GetMock<IScooterService>();
            _scooterserviceMock.Setup(r => r.GetScooters()).Returns(new List<Scooter> { scooter1, scooter2, scooter3 });

            var _recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter1, rentedScooter2, rentedScooter3 });

            PriceCalculation priceCalculation = new PriceCalculation(_recordsServiceMock.Object);

            RentalCompany company = new RentalCompany(_companyName, _scooterserviceMock.Object, _recordsServiceMock.Object, priceCalculation);

            decimal income = company.CalculateIncome(null, true);
            income.Should().BeGreaterThan(80);
        }

        [TestMethod]
        public void CalculateTotalIncome_ForAllPeriodIncludedOnlyFinishedRents_CalculateIncome()
        {
            string id1 = "1";
            DateTime rentStart1 = new DateTime(2020, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd1 = rentStart1.AddDays(2);
            decimal pricePerMinute1 = 1m;

            string id2 = "2";
            DateTime rentStart2 = new DateTime(2021, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd2 = rentStart2.AddDays(2);
            decimal pricePerMinute2 = 1m;

            string id3 = "3";
            DateTime rentStart3 = new DateTime(2023, 9, 10, 2, 0, 0, 0);
            decimal pricePerMinute3 = 1m;

            var rentedScooter1 = new RentedScooter(id1, rentStart1, pricePerMinute1)
            {
                RentEnd = rentEnd1
            };
            var scooter1 = new Scooter(id1, pricePerMinute1);


            var rentedScooter2 = new RentedScooter(id2, rentStart2, pricePerMinute2)
            {
                RentEnd = rentEnd2
            };
            var scooter2 = new Scooter(id2, pricePerMinute2);

            var rentedScooter3 = new RentedScooter(id3, rentStart3, pricePerMinute3)
            {
                RentEnd = null
            };
            var scooter3 = new Scooter(id3, pricePerMinute3);

            var _scooterserviceMock = _mocker.GetMock<IScooterService>();
            _scooterserviceMock.Setup(r => r.GetScooters()).Returns(new List<Scooter> { scooter1, scooter2, scooter3 });

            var _recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter1, rentedScooter2, rentedScooter3 });

            PriceCalculation priceCalculation = new PriceCalculation(_recordsServiceMock.Object);

            RentalCompany company = new RentalCompany(_companyName, _scooterserviceMock.Object, _recordsServiceMock.Object, priceCalculation);

            decimal income = company.CalculateIncome(null, false);
            income.Should().Be(80);
        }

        [TestMethod]
        public void CalculateTotalIncome_ForYearIncludedOnlyFinishedRents_CalculateIncome()
        {
            string id1 = "1";
            DateTime rentStart1 = new DateTime(2020, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd1 = rentStart1.AddDays(2);
            decimal pricePerMinute1 = 1m;

            string id2 = "2";
            DateTime rentStart2 = new DateTime(2021, 1, 1, 0, 0, 0, 0);
            DateTime rentEnd2 = rentStart2.AddDays(2);
            decimal pricePerMinute2 = 1m;

            string id3 = "3";
            DateTime rentStart3 = new DateTime(2023, 9, 10, 2, 0, 0, 0);
            decimal pricePerMinute3 = 1m;

            var rentedScooter1 = new RentedScooter(id1, rentStart1, pricePerMinute1)
            {
                RentEnd = rentEnd1
            };
            var scooter1 = new Scooter(id1, pricePerMinute1);

            var rentedScooter2 = new RentedScooter(id2, rentStart2, pricePerMinute2)
            {
                RentEnd = rentEnd2
            };
            var scooter2 = new Scooter(id2, pricePerMinute2);

            var rentedScooter3 = new RentedScooter(id3, rentStart3, pricePerMinute3)
            {
                RentEnd = null
            };
            var scooter3 = new Scooter(id3, pricePerMinute3);

            var _scooterserviceMock = _mocker.GetMock<IScooterService>();
            _scooterserviceMock.Setup(r => r.GetScooters()).Returns(new List<Scooter> { scooter1, scooter2, scooter3 });

            var _recordsServiceMock = _mocker.GetMock<IRentalRecordsService>();
            _recordsServiceMock.Setup(r => r.GetRentedScooterList()).Returns(new List<RentedScooter> { rentedScooter1, rentedScooter2, rentedScooter3 });

            PriceCalculation priceCalculation = new PriceCalculation(_recordsServiceMock.Object);

            RentalCompany company = new RentalCompany(_companyName, _scooterserviceMock.Object, _recordsServiceMock.Object, priceCalculation);

            decimal income = company.CalculateIncome(2020, false);
            income.Should().Be(40);
        }
    }
}