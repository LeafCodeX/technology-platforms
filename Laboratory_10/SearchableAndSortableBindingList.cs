using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Laboratory_10.Classes;

namespace Laboratory_10
{
    // Klasa dziedzicząca z BindingList<Car> z dodatkowymi funkcjami wyszukiwania i sortowania
    public class SearchableAndSortableBindingList : BindingList<Car>
    {
        // Pola przechowujące informacje o ostatnio użytych sortowaniach
        private bool _bModel = false;
        private bool _bYear = false;
        private bool _bMotor = false;

        // Konstruktor przyjmujący listę samochodów
        public SearchableAndSortableBindingList(List<Car> cars) : base(cars)
        {
        }

        // Metoda do wyszukiwania samochodów na podstawie wybranego kryterium
        public List<Car> Find(string text, string combo)
        {
            var matchingCars = new List<Car>();

            // Przechodzenie przez każdy samochód w liście
            foreach (var car in this)
            {
                // Sprawdzenie, które kryterium wyszukiwania zostało wybrane
                switch (combo)
                {
                    // Wyszukiwanie po modelu
                    case "Model":
                        if (car.Model == text)
                        {
                            matchingCars.Add(car);
                        }
                        break;

                    // Wyszukiwanie po roku
                    case "Year":
                        if (car.Year == int.Parse(text))
                        {
                            matchingCars.Add(car);
                        }
                        break;

                    // Wyszukiwanie po modelu silnika
                    case "Motor":
                        if (car.Motor.Model == text)
                        {
                            matchingCars.Add(car);
                        }
                        break;
                }
            }

            return matchingCars;
        }

        // Metoda dodająca nowy samochód do listy
        public List<Car> AddElement(string model, string engineModel, double horsepower, double displacement, int year)
        {
            var newCar = new Car(model, new Engine(displacement, horsepower, engineModel), year);
            Add(newCar);
            return this.ToList();
        }

        // Metoda sortująca listę samochodów na podstawie wybranego kryterium
        public List<Car> Sort(string property)
        {
            var matchingCars = this.ToList();

            // Wybór kryterium sortowania
            switch (property)
            {
                // Sortowanie po modelu
                case "Model":
                    _bModel = !_bModel;
                    return _bModel
                        ? matchingCars.OrderBy(car => car.Model).ToList()
                        : matchingCars.OrderByDescending(car => car.Model).ToList();

                // Sortowanie po roku
                case "Year":
                    _bYear = !_bYear;
                    return _bYear
                        ? matchingCars.OrderBy(car => car.Year).ToList()
                        : matchingCars.OrderByDescending(car => car.Year).ToList();

                // Sortowanie po modelu silnika
                case "Motor":
                    _bMotor = !_bMotor;
                    return _bMotor
                        ? matchingCars.OrderBy(car => car.Motor.Model).ToList()
                        : matchingCars.OrderByDescending(car => car.Motor.Model).ToList();

                // Domyślne zachowanie - zwrócenie niezmienionej listy
                default:
                    return matchingCars;
            }
        }
    }
}
