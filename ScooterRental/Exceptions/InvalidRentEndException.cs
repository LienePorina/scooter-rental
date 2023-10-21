namespace ScooterRental.Exceptions
{
    public class InvalidRentEndException : Exception
    {
        public InvalidRentEndException() : base("RentEnd cannot be earlier than RentStart")
        { 
        }
    }
}
