namespace ScooterRental.Exceptions
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException() : base("Id can not be null or empty")
        {
        }
    }
}
