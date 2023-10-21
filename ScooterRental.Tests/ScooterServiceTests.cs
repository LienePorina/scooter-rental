using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class ScooterServiceTests
    {
        private IScooterService _scooterService;
        private List<Scooter> _scooterStorage;
        private const string DEFAULT_SCOOTER_ID = "1";
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;

        [TestInitialize]
        public void Setup()
        {
            _scooterStorage = new List<Scooter>();
            _scooterService = new ScooterService(_scooterStorage);
        }

        [TestMethod]
        public void Constructor_WithValidArguments_ShouldInitializeScooters()
        {
            _scooterService.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_WithNullArgument_ShouldThrowScooterStorageIsNullException()
        {
            List<Scooter> _scooterStorage = null;

            Action action = () => new ScooterService(_scooterStorage);

            action.Should().Throw<ScooterStorageIsNullException>();
        }

        [TestMethod]
        public void AddScooter_WirhIdAndPricePerMinute_ScooterAdded()
        {
            _scooterService.AddScooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddScooter_WirhId1AndPricePerMinute1_ScooterAddedWithId1AndPrice1()
        {
            _scooterService.AddScooter(DEFAULT_SCOOTER_ID, 1m);

            var scooter = _scooterStorage.First();

            scooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            scooter.PricePerMinute.Should().Be(1m);
        }

        [TestMethod]
        public void AddScooter_AddDuplicateScooter_ThrowsDuplicateScooterException()
        {
            _scooterStorage.Add(new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE));

            Action action = () => _scooterService.AddScooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<DuplicateScooterException>();
        }

        [TestMethod]
        public void AddScooter_AddScooterWithNegativePrice_ThrowsNegativePriceException()
        {
            Action action = () => _scooterService.AddScooter(DEFAULT_SCOOTER_ID, -1);

            action.Should().Throw<NegativePriceException>();
        }

        [TestMethod]
        public void AddScooter_AddScooterWithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.AddScooter("", DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void RemoveScooter_WirhId_ScooterRemoved()
        {
            var _scooterStorage = new List<Scooter>
            {
                new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE),
            };
            _scooterService = new ScooterService(_scooterStorage);

            _scooterService.RemoveScooter(DEFAULT_SCOOTER_ID);

            _scooterStorage.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveScooter_WirhIdNotFound_ThrowsScooterNotFoundException()
        {
            Action action = () => _scooterService.RemoveScooter(DEFAULT_SCOOTER_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void RemoveScooter_RemoveScooterWithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.RemoveScooter("");

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void GetScooters_ReturnsScootersList()
        {
            var _scooterStorage = new List<Scooter>
            {
                new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE),
            };

            var scooterService = new ScooterService(_scooterStorage);

            var scooters = scooterService.GetScooters();

            scooters.Should().NotBeNullOrEmpty();
            scooters.Count.Should().Be(1);
        }

        [TestMethod]
        public void GetScooters_ReturnsEmptyListWhenNoScooters()
        {
            var scooters = _scooterService.GetScooters();

            scooters.Should().NotBeNull();
            scooters.Count.Should().Be(0);
        }

        [TestMethod]
        public void GetScooters_ReturnsScootersListWithScooterProperties()
        {
            var _scooterStorage = new List<Scooter>
            {
                new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE),
            };

            var scooterService = new ScooterService(_scooterStorage);

            var scooters = scooterService.GetScooters();

            scooters[0].Id.Should().Be(DEFAULT_SCOOTER_ID);
            scooters[0].PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void GetScooterById_ReturnsScooter()
        {
            var _scooterStorage = new List<Scooter>
            {
                new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE),
            };

            var _scooterService = new ScooterService(_scooterStorage);

            var scooter = _scooterService.GetScooterById(DEFAULT_SCOOTER_ID);

            scooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            scooter.PricePerMinute.Should().Be(DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void GetScooterById_ScooterStorageIsNull_ThrowsScooterNotFoundException()
        {
            List<Scooter> _scooterStorage = null;

            Action action = () => _scooterService.GetScooterById(DEFAULT_SCOOTER_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void GetScooterById_ScooterNotExists_ThrowsScooterNotFoundException()
        {
            Action action = () => _scooterService.GetScooterById(DEFAULT_SCOOTER_ID);

            action.Should().Throw<ScooterNotFoundException>();
        }
    }
}