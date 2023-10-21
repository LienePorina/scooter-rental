namespace ScooterRental
{
    public class RentalRecordsService : IRentalRecordsService
    {
        private readonly List<RentedScooter> _rentedScooterList;

        public RentalRecordsService(List<RentedScooter> rentedScooterList)
        {
            _rentedScooterList = rentedScooterList;
        }

        public List<RentedScooter> GetRentedScooterList()
        {
            return _rentedScooterList;
        }

        public void StartRent(string id, DateTime rentStart, decimal pricePerMinute)
        {
            if (pricePerMinute <= 0)
            {
                throw new Exceptions.NegativePriceException();
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }

            _rentedScooterList.Add(new RentedScooter(id, DateTime.Now, pricePerMinute));
        }

        public RentedScooter StopRent(string id, DateTime rentEnd)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }

            var rentalRecord = _rentedScooterList.FirstOrDefault(s => s.Id == id && !s.RentEnd.HasValue);
            rentalRecord.RentEnd = rentEnd;

            return rentalRecord;
        }
    }
}