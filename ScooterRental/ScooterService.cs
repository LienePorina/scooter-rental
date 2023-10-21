using ScooterRental.Exceptions;

namespace ScooterRental
{
    public class ScooterService : IScooterService
    {
        private readonly List<Scooter> _scooters;

        public ScooterService(List<Scooter> scooterStorage)
        {
            if (scooterStorage == null)
            {
                throw new Exceptions.ScooterStorageIsNullException();
            }
            _scooters = scooterStorage;
        }

        public void AddScooter(string id, decimal pricePerMinute)
        {
            if (_scooters.Any(s => s.Id == id))
            {
                throw new Exceptions.DuplicateScooterException();
            }

            if (pricePerMinute <= 0)
            {
                throw new Exceptions.NegativePriceException();
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }

            _scooters.Add(new Scooter(id, pricePerMinute));
        }

        public void RemoveScooter(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exceptions.InvalidIdException();
            }

            Scooter scooterToRemove = _scooters.FirstOrDefault(s => s.Id == id);

            if (scooterToRemove != null)
            {
                _scooters.Remove(scooterToRemove);
            }
            else
            {
                throw new Exceptions.ScooterNotFoundException();
            }
        }

        public IList<Scooter> GetScooters()
        {
            return _scooters.ToList();
        }

        public Scooter GetScooterById(string scooterId)
        {
            var scooter = _scooters.FirstOrDefault(s => s.Id == scooterId);

            if (scooter == null)
            {
                throw new Exceptions.ScooterNotFoundException();
            }

            return scooter;
        }
    }
}