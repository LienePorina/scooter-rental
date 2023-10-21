using ScooterRental.Interfaces;

namespace ScooterRental
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IScooterService _scooterService;
        private readonly IRentalRecordsService _rentalRecordsService;
        private readonly IPriceCalculation _priceCalculation;

        public RentalCompany(string name, IScooterService scooterService, IRentalRecordsService rentalRecordsService, IPriceCalculation priceCalculation)
        {
            Name = name;
            _scooterService = scooterService;
            _rentalRecordsService = rentalRecordsService;
            _priceCalculation = priceCalculation;
        }

        public string Name { get; }

        private void ValidateScooterId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }

            var scooter = _scooterService.GetScooterById(id);

            if (scooter == null)
            {
                throw new Exceptions.ScooterNotFoundException();
            }
        }

        public void StartRent(string id)
        {
            ValidateScooterId(id);

            var scooter = _scooterService.GetScooterById(id);

            scooter.IsRented = true;

            _rentalRecordsService.StartRent(scooter.Id, DateTime.Now, scooter.PricePerMinute);
        }

        public decimal EndRent(string id)
        {
            ValidateScooterId(id);

            var scooter = _scooterService.GetScooterById(id);

            var rentedScooterList = _rentalRecordsService.GetRentedScooterList();
            var rentedScooter = rentedScooterList.FirstOrDefault(s => s.Id == id && s.RentEnd != null);

            if (rentedScooter == null || !scooter.IsRented)
            {
                throw new Exceptions.ScooterNotFoundException();
            }

            scooter.IsRented = false;
            var rentalRecord = _rentalRecordsService.StopRent(scooter.Id, DateTime.Now);

            return _priceCalculation.CalculateIncomeFromOneRent(id);
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            decimal income = _priceCalculation.CalculateTotalIncome(year, includeNotCompletedRentals);

            return income;
        }
    }
}