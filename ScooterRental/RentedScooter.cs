namespace ScooterRental
{
    public class RentedScooter
    {
        public RentedScooter(string id, DateTime startTime, decimal pricePerMinute)
        {
            if (pricePerMinute <= 0)
            {
                throw new Exceptions.NegativePriceException();
            }

            Id = id;
            RentStart = startTime;
            PricePerMinute = pricePerMinute;
        }

        public string Id { get; }

        public DateTime RentStart { get; }

        public DateTime? rentEnd { get; set; }

        public DateTime? RentEnd
        {
            get { return rentEnd; }

            set
            {
                if (value.HasValue && value <= RentStart)
                {
                    throw new Exceptions.InvalidRentEndException();
                }
                rentEnd = value;
            }
        }

        public decimal PricePerMinute { get; }
    }
}