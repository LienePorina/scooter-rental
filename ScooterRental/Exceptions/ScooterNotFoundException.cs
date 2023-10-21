namespace ScooterRental.Exceptions
{
    public class ScooterNotFoundException : Exception
    {
        public ScooterNotFoundException() : base("Scooter not found")
        {
        }
    }
}