namespace ScooterRental.Exceptions
{
    public class NegativePriceException : Exception
    {
        public NegativePriceException() : base("Price can not be negatrive")
        {
        }
    }
}
