namespace ScooterRental.Interfaces
{
    public interface IPriceCalculation
    {
        (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsForOneCompletedRent(string id);

        (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsForOneNotCompletedRent(string id);

        RentedScooter GetRentedScooter(string id);

        (int firstDayMinutes, int lastDayMinutes, int daysBetween) CalculateRentDetailsInternal(RentedScooter rentedScooter);

        decimal CalculateIncomeFromOneRent(string id);

        public decimal CalculateTotalIncome(int? year, bool includeNotCompletedRentals);
    }
}