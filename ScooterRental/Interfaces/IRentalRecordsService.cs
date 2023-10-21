namespace ScooterRental
{
    public interface IRentalRecordsService
    {
        public List<RentedScooter> GetRentedScooterList();

        public void StartRent(string id, DateTime rentStart, decimal pricePerMinute);

        public RentedScooter StopRent(string id, DateTime rentEnd);
    }
}