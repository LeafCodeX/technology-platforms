using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Laboratory_10.Classes;
using Laboratory_10;
using System.Collections;

namespace Laboratory_10
{
    public partial class MainWindow : Window
    {
        private static readonly List<Car> MyCars = new List<Car>
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };

        private List<Car> _tempCars;
        private BindingList<Car> _myCarsBindingList;
        private SearchableAndSortableBindingList _carList = new SearchableAndSortableBindingList(MyCars);

        public MainWindow()
        {
            InitializeComponent();
            InitializeComboBox();
            BindDataToGrid(MyCars);

            string queryExpressionResult = QueryExpression();
            string methodBasedResult = MethodBased();
            string taskResult = Task();

            string combinedResult = queryExpressionResult + "\n" + methodBasedResult + "\n" + taskResult;

            MessageBox.Show(combinedResult, "Combined Results");
        }

        private void InitializeComboBox()
        {
            ComboBox.Items.Add("Model");
            ComboBox.Items.Add("Motor");
            ComboBox.Items.Add("Year");
        }

        private void BindDataToGrid(List<Car> cars)
        {
            _myCarsBindingList = new BindingList<Car>(cars);
            CarsDataGrid.ItemsSource = _myCarsBindingList;
        }

        private static string Task()
        {
            MyCars.Sort((car1, car2) => car1.Motor.Horsepower.CompareTo(car2.Motor.Horsepower));
            StringBuilder taskResult = new StringBuilder("[INFO] Task Output:\n");
            MyCars.FindAll(car => car.Motor.Model == "TDI").ForEach(car =>
                taskResult.AppendLine($" >> Model: {car.Model}\n >> Silnik: {car.Motor}\n >> Rok: {car.Year}\n"));
            return taskResult.ToString();
        }

        public void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
                return;

            _tempCars = _carList.ToList().Where(x => x != (Car)(sender as DataGrid).SelectedItem).ToList();
            _carList = new SearchableAndSortableBindingList(_tempCars);
            BindDataToGrid(_tempCars);
        }

        private void Search_Button(object sender, RoutedEventArgs e)
        {
            var query = SearchTextBox.Text;
            if (ComboBox.SelectedItem == null) return;
            var property = ComboBox.SelectedItem.ToString();

            _tempCars = _carList.Find(query, property);
            BindDataToGrid(_tempCars);
        }

        public void Add_Button(object sender, RoutedEventArgs e)
        {
            var model = Model.Text;
            var engineModel = EngineModel.Text;
            var horsepower = float.Parse(Horsepower.Text);
            var displacement = float.Parse(Displacement.Text);
            var year = int.Parse(Year.Text);

            _tempCars = _carList.AddElement(model, engineModel, horsepower, displacement, year);
            _carList = new SearchableAndSortableBindingList(_tempCars);
            BindDataToGrid(_tempCars);
        }

        private void Sort_Model(object sender, RoutedEventArgs e)
        {
            _tempCars = _carList.Sort("Model");
            BindDataToGrid(_tempCars);
        }

        private void Sort_Year(object sender, RoutedEventArgs e)
        {
            _tempCars = _carList.Sort("Year");
            BindDataToGrid(_tempCars);
        }

        private void Sort_Motor(object sender, RoutedEventArgs e)
        {
            _tempCars = _carList.Sort("Motor");
            BindDataToGrid(_tempCars);
        }

        private void Reset_Button(object sender, RoutedEventArgs e)
        {
            BindDataToGrid(_carList.ToList());
        }

        private static string QueryExpression()
        {
            var result = from c in MyCars
                         where c.Model == "A6"
                         let engineType = c.Motor.Model == "TDI" ? "diesel" : "petrol"
                         let hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
                         group hppl by engineType into g
                         orderby g.Average() descending
                         select new
                         {
                             engineType = g.Key,
                             avgHPPL = g.Average()
                         };

            return result.Aggregate("[INFO] Query Expression Output: \n", (current, e) => current + " >> " + (e.engineType + ": " + e.avgHPPL + " \n"));
        }

        private static string MethodBased()
        {
            var result = MyCars
                .Where(c => c.Model == "A6")
                .Select(c => new
                {
                    engineType = c.Motor.Model == "TDI" ? "diesel" : "petrol",
                    hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
                })
                .GroupBy(c => c.engineType)
                .Select(g => new
                {
                    engineType = g.Key,
                    avgHPPL = g.Average(c => c.hppl)
                })
                .OrderByDescending(c => c.avgHPPL);

            return result.Aggregate("[INFO] Method-based Query Output: \n", (current, e) => current + " >> " + (e.engineType + ": " + e.avgHPPL + " \n"));
        }
    }
}
