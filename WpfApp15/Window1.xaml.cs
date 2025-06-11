using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;

namespace WpfApp15
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public class Participant
        {
            public int ID { get; set; } // свойство ID для нумерации
            public string FullName { get; set; }
            public DateTime BirthDate { get; set; }
            public string BirthDateString => BirthDate.ToString("dd/MM/yyyy");
            public string Country { get; set; }
            public string Instrument { get; set; }
            public int Age { get; set; }
        }

        private List<Participant> participants = new List<Participant>();
        private List<Participant> filteredParticipants = new List<Participant>();
        private int nextID = 1;

        private List<string> results = new List<string>();

        public Window1()
        {
            InitializeComponent();
            ParticipantsDataGrid.ItemsSource = participants;
        }

        private void AddParticipant_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string birthDateStr = BirthDateTextBox.Text.Trim();
            string country = CountryTextBox.Text.Trim();
            string instrument = (InstrumentComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(birthDateStr) ||
                string.IsNullOrEmpty(country) || string.IsNullOrEmpty(instrument))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (!DateTime.TryParseExact(birthDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime birthDate))
            {
                MessageBox.Show("Некорректный формат даты. Используйте дд/мм/гггг.");
                return;
            }

            int age = CalculateAge(birthDate);

            var participant = new Participant
            {
                ID = nextID++,
                FullName = fullName,
                BirthDate = birthDate,
                Country = country,
                Instrument = instrument,
                Age = age
            };

            participants.Add(participant);
            RefreshDataGrid();

            // Очистка полей
            FullNameTextBox.Clear();
            BirthDateTextBox.Clear();
            CountryTextBox.Clear();
            InstrumentComboBox.SelectedIndex = -1;

            MessageBox.Show($"Участник {fullName} добавлен. Возраст: {age} лет.");
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
                age--;
            return age;
        }

        private void RefreshDataGrid()
        {
            ParticipantsDataGrid.ItemsSource = null;
            ParticipantsDataGrid.ItemsSource = participants;
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(participants, options);
                File.WriteAllText("participants.json", json);
                MessageBox.Show("Данные успешно сохранены в файл.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists("participants.json"))
                {
                    MessageBox.Show("Файл не найден.");
                    return;
                }
                string json = File.ReadAllText("participants.json");
                var loadedParticipants = JsonSerializer.Deserialize<List<Participant>>(json);
                if (loadedParticipants != null)
                {
                    participants.Clear();
                    participants.AddRange(loadedParticipants);
                    if (participants.Count > 0)
                        nextID = participants.Max(p => p.ID) + 1;
                    else
                        nextID = 1;

                    RefreshDataGrid();
                    MessageBox.Show("Данные успешно загружены из файла.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ParticipantsDataGrid.SelectedItem is Participant selectedParticipant)
            {
                participants.Remove(selectedParticipant);
                RefreshDataGrid();
                MessageBox.Show("Участник удален.");
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите участника для удаления.");
            }
        }

        private void EditSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ParticipantsDataGrid.SelectedItem is Participant selectedParticipant)
            {
                // Заполняем поля формы выбранным участником для редактирования
                FullNameTextBox.Text = selectedParticipant.FullName;
                BirthDateTextBox.Text = selectedParticipant.BirthDateString;
                CountryTextBox.Text = selectedParticipant.Country;

                // Удаляем старого участника из списка
                participants.Remove(selectedParticipant);
                RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите участника для редактирования.");
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FilterAgeTextBox.Text.Trim(), out int filterAge))
            {
                filteredParticipants = participants.Where(p => p.Age == filterAge).ToList();
                ParticipantsDataGrid.ItemsSource = filteredParticipants;
            }
            else
            {
                MessageBox.Show("Введите корректный возраст для фильтрации.");
            }
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ParticipantsDataGrid.ItemsSource = participants;
            FilterAgeTextBox.Clear();
        }

        private void ShowAllParticipants_Click(object sender, RoutedEventArgs e)
        {
            ParticipantsDataGrid.ItemsSource = participants;
        }

        private void SaveResultsToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultsJson = JsonSerializer.Serialize(results);
                File.WriteAllText("results.json", resultsJson);
                MessageBox.Show("Результаты сохранены в файл.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении результатов: {ex.Message}");
            }
        }

        private void FindLaureates_Click(object sender, RoutedEventArgs e)
        {
            var laureates = participants.Where(p => p.Age >= 18).ToList();

            results.Clear();

            if (laureates.Count == 0)
            {
                results.Add("Нет лауреатов по заданным критериям.");
            }
            else
            {
                results.Add($"Лауреаты ({laureates.Count}):");
                foreach (var p in laureates)
                {
                    results.Add($"{p.FullName} - Возраст: {p.Age}");
                }
            }

            // Вывод результатов
            ResultsListBox.ItemsSource = null;
            ResultsListBox.ItemsSource = results;
        }


    }
}
