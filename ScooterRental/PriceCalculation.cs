using ScooterRental.Interfaces;

namespace ScooterRental
{
    public class PriceCalculation : IPriceCalculation
    {
        private readonly IRentalRecordsService _rentalRecordsService;

        public PriceCalculation(IRentalRecordsService rentalRecordsService)
        {
            _rentalRecordsService = rentalRecordsService;
        }

        private void ValidateId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }
        }

        public (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsForOneCompletedRent(string id)
        {
            ValidateId(id);

            RentedScooter rentedScooter = GetRentedScooter(id);

            if (rentedScooter.RentEnd.HasValue)
            {
                return CalculateRentDetailsInternal(rentedScooter);
            }
            else
            {
                return (0, 0, 0);
            }
        }

        public (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsForOneNotCompletedRent(string id)
        {
            ValidateId(id);

            RentedScooter rentedScooter = GetRentedScooter(id);

            if (!rentedScooter.RentEnd.HasValue)
            {
                rentedScooter.RentEnd = DateTime.Now;
                return CalculateRentDetailsInternal(rentedScooter);
            }
            else
            {
                return (0, 0, 0);
            }
        }

        public RentedScooter GetRentedScooter(string id)
        {
            ValidateId(id);

            List<RentedScooter> rentedScooters = _rentalRecordsService.GetRentedScooterList();
            RentedScooter rentedScooter = rentedScooters.FirstOrDefault(s => s.Id == id);

            if (rentedScooter != null)
            {
                return rentedScooter;
            }
            else
            {
                throw new Exceptions.ScooterNotFoundException();
            }
        }

        public (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsInternal(RentedScooter rentedScooter)
        {
            TimeSpan rentalDuration = rentedScooter.RentEnd.Value - rentedScooter.RentStart;
            int daysBetween = (int)rentalDuration.TotalDays;

            int firstDayMinutes = 0;
            int lastDayMinutes = 0;

            if (daysBetween >= 1)
            {
                firstDayMinutes = (int)(24 * 60 - rentedScooter.RentStart.TimeOfDay.TotalMinutes);
                lastDayMinutes = (int)rentedScooter.RentEnd.Value.TimeOfDay.TotalMinutes;
            }
            else
            {
                firstDayMinutes = (int)rentalDuration.TotalMinutes;
            }

            return (firstDayMinutes, lastDayMinutes, daysBetween);
        }

        public decimal CalculateIncomeFromOneRent(string id)
        {
            RentedScooter rentedScooter = _rentalRecordsService.GetRentedScooterList().FirstOrDefault(s => s.Id == id);

            if (rentedScooter == null)
            {
                return 0;
            }

            var (firstDayMinutes, lastDayMinutes, daysBetween) = rentedScooter.RentEnd.HasValue
                ? CalculateRentDetailsForOneCompletedRent(id)
                : CalculateRentDetailsForOneNotCompletedRent(id);

            decimal pricePerMinute = rentedScooter.PricePerMinute;
            decimal income = 0;

            if (daysBetween >= 1)
            {
                income += Math.Min(20, 24 * 60 * pricePerMinute) * (daysBetween - 1);
            }

            income += Math.Min(20, firstDayMinutes * pricePerMinute);
            income += Math.Min(20, lastDayMinutes * pricePerMinute);

            return income;
        }

        public decimal CalculateTotalIncome(int? year, bool includeNotCompletedRentals)
        {
            var rentedScooterList = _rentalRecordsService.GetRentedScooterList();
            decimal income = 0;
            if (!year.HasValue && includeNotCompletedRentals)
            {
                income = rentedScooterList.Select(rental => CalculateIncomeFromOneRent(rental.Id)).Sum();
            }
            else if (!year.HasValue && !includeNotCompletedRentals)
            {
                var filtredRentals = rentedScooterList.Where(rental => rental.RentEnd.HasValue);
                income = filtredRentals.Select(rental => CalculateIncomeFromOneRent(rental.Id)).Sum();
            }
            else if (year.HasValue && !includeNotCompletedRentals)
            {
                var filtredRentals = rentedScooterList.Where(rental => rental.RentEnd.HasValue);
                var filtredByYear = filtredRentals.Where(rental => rental.RentStart.Year == year);
                income = filtredByYear.Select(rental => CalculateIncomeFromOneRent(rental.Id)).Sum();
            }

            return income;
        }
    }
}