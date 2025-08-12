using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2_Formularz
{
    public partial class MainWindow : Window
    {
        private Positions _positions = new Positions();            //Uzycie stanowisk z klasy Positions zamiast stanowisk zdefiniowanych w xaml Combo Box
        private Team _team = new Team("Team 1");

        public MainWindow()
        {
            InitializeComponent();

            //Podpięcie stanowisk pod combo boxa
            cb_stanowisko.ItemsSource = _positions;
            //cb_stanowisko.DisplayMemberPath = "Name"; gdyby stanowiska były pojedynczymi obiektami z własnością Name na liście Stanowisk (ale są zwykłymi stringami)

            //Podpięcie pracowników pod List Boxa
            lb_pracownicy.ItemsSource = _team.Staff;
        }

        
        private void btn_dodaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbx_imie.Text) ||
                string.IsNullOrEmpty(tbx_nazwisko.Text) || dp_data.SelectedDate == null ||
                cb_stanowisko.SelectedItem == null || string.IsNullOrEmpty(tbx_pensja.Text) || (rb_B2B.IsChecked==false
                && rb_UOP.IsChecked==false && rb_UZ.IsChecked==false) )
            {
                MessageBox.Show("Pola muszą być wypełnione", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string contract = "";
            if (rb_UOP.IsChecked == true)
                contract = "Umowa o pracę";
            else if (rb_B2B.IsChecked == true)
                contract = "Umowa B2B";
            else if (rb_UZ.IsChecked == true)
                contract = "Umowa zlecenie";

            Employee employee = new Employee(tbx_imie.Text, tbx_nazwisko.Text,
                (DateTime)dp_data.SelectedDate,(string)cb_stanowisko.SelectedItem,tbx_pensja.Text, contract);

            _team.AddEmployee(employee);


            //Wyczyszczenie formularza
            tbx_imie.Clear();
            tbx_nazwisko.Clear();
            dp_data.SelectedDate = null;
            cb_stanowisko.SelectedItem = null;
            tbx_pensja.Clear();
            rb_UOP.IsChecked = false;
            rb_B2B.IsChecked = false;
            rb_UZ.IsChecked = false;
        }

        private void tbx_imie_PreviewTextInput(object sender,
            TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsLetter);         //Do imienia mozna wpisac tylko litery
        }

        private void tbx_pensja_PreviewTextInput(object sender,
            TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);          //Do pensji mozna wpisac tylko cyfry
        }

        
        private void btn_usun_Click(object sender, RoutedEventArgs e)
        {
            if (lb_pracownicy.SelectedItem is Employee selectedEmployee)
            {
                _team.RemoveEmployee(selectedEmployee);
            }
            else
            {
                MessageBox.Show("Wybierz pracownika do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_edytuj_Click(object sender, RoutedEventArgs e)
        {
            if (lb_pracownicy.SelectedItem is Employee selectedEmployee)
            {
                tbx_imie.Text = selectedEmployee.first_name;
                tbx_nazwisko.Text = selectedEmployee.last_name;
                dp_data.SelectedDate = selectedEmployee.birth_date;
                cb_stanowisko.SelectedItem = selectedEmployee.job;
                tbx_pensja.Text = selectedEmployee.salary;

                if (selectedEmployee.contract_type == "Umowa o pracę")
                    rb_UOP.IsChecked = true;
                else if (selectedEmployee.contract_type == "Umowa B2B")
                    rb_B2B.IsChecked = true;
                else if (selectedEmployee.contract_type == "Umowa zlecenie")
                    rb_UZ.IsChecked = true;

                _team.RemoveEmployee(selectedEmployee);
            }
            else
            {
                MessageBox.Show("Wybierz pracownika do edycji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btn_zapisz_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Zapisz plik"
            };

            string text = _team.ToString();

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, text);
                MessageBox.Show("Plik zapisany pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_wczytaj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Zapis pracownikow"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                /*string lines = File.ReadAllText(openFileDialog.FileName);
                MessageBox.Show(lines);'*/   //Dla wyświetlenia w MessageBoxie

                string[] lines = File.ReadAllLines(openFileDialog.FileName);

                foreach (string line in lines)
                {
                    Employee emp = ParseEmployeeFromString(line);
                    if (emp != null)
                        _team.AddEmployee(emp);
                }

                MessageBox.Show("Wczytano pracowników z pliku.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        
        }

        private Employee ParseEmployeeFromString(string line)
        {
            try
            {
                // Przykład linii:
                // Jan Kowalski, wiek: 32, stanowiska: Developer, pensja: 8000, typ umowy: Umowa o pracę

                string[] parts = line.Split(',');

                string[] imieNazwisko = parts[0].Trim().Split(' ');
                string nazwisko = imieNazwisko[^1];
                string imie;
                if(imieNazwisko.Length < 3)
                {
                    imie = imieNazwisko[0];
                }
                else 
                {
                    imie = string.Join(" ", imieNazwisko.Take(imieNazwisko.Length - 1));
                }
                
                int wiek = int.Parse(parts[1].Trim().Replace("wiek: ", ""));
                string stanowisko = parts[2].Trim().Replace("stanowiska: ", "");
                string pensja = parts[3].Trim().Replace("pensja: ", "");
                string umowa = parts[4].Trim().Replace("typ umowy: ", "");

                // Przybliżona data urodzenia
                DateTime dataUrodzenia = DateTime.Now.AddYears(-wiek);

                return new Employee(imie, nazwisko, dataUrodzenia, stanowisko, pensja, umowa);
            }
            catch
            {
                // Jeśli nie uda się sparsować — pomiń linię
                return null;
            }
        }




    }
}