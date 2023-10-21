namespace ScooterRental.Exceptions
{
    public class ScooterStorageIsNullException : Exception
    {
        public ScooterStorageIsNullException() : base("Scooter storage is null")
        { 
        }
    }
}