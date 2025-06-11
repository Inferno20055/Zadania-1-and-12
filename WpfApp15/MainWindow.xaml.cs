using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.IO;

namespace WpfApp15
{
    ///пришлось использовать сериализацию для сохранения в json и xaml файл
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class BusSchedule
        {
            public string BusNumber { get; set; }
            public string BusType { get; set; }
            public string Destination { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }

            public DateTime CreationDate { get; private set; }

            public BusSchedule()
            {
                CreationDate = DateTime.Now;
            }
        }
        private ObservableCollection<BusSchedule> schedules = new ObservableCollection<BusSchedule>();

        public MainWindow()
        {
            InitializeComponent();
            dataGrid.ItemsSource = schedules;
        }

        // Сохранение данных в файл
        private void SaveToFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (saveFileDialog.FilterIndex == 1)
                    {
                        // JSON сериализация
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        var json = JsonSerializer.Serialize(schedules, options);
                        File.WriteAllText(saveFileDialog.FileName, json);
                    }
                    else
                    {
                        // XML сериализация
                        var serializer = new XmlSerializer(typeof(ObservableCollection<BusSchedule>));
                        using (var stream = File.OpenWrite(saveFileDialog.FileName))
                        {
                            serializer.Serialize(stream, schedules);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                }
            }
        }

        // Загрузка данных из файла
        private void LoadFromFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (openFileDialog.FilterIndex == 1)
                    {
                        // JSON десериализация
                        var json = File.ReadAllText(openFileDialog.FileName);
                        var loadedSchedules = JsonSerializer.Deserialize<ObservableCollection<BusSchedule>>(json);
                        schedules.Clear();
                        foreach (var item in loadedSchedules)
                            schedules.Add(item);
                    }
                    else
                    {
                        // XML десериализация
                        var serializer = new XmlSerializer(typeof(ObservableCollection<BusSchedule>));
                        using (var stream = File.OpenRead(openFileDialog.FileName))
                        {
                            var loadedSchedules = (ObservableCollection<BusSchedule>)serializer.Deserialize(stream);
                            schedules.Clear();
                            foreach (var item in loadedSchedules)
                                schedules.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}");
                }
            }
        }


        private void AddNewSchedule()
        {
            var dialog = new ScheduleEditWindow();
            if (dialog.ShowDialog() == true)
            {
                schedules.Add(dialog.BusSchedule);
            }
        }

        private void FilterSchedules(string destination, DateTime beforeArrival)
        {
            var filtered = new ObservableCollection<BusSchedule>();
            foreach (var schedule in schedules)
            {
                if (schedule.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                    schedule.ArrivalDateTime <= beforeArrival)
                {
                    filtered.Add(schedule);
                }
            }

            dataGrid.ItemsSource = filtered;
        }

        // Обработчики кнопок меню или команд

        private void SaveMenu_Click(object sender, RoutedEventArgs e) => SaveToFile();

        private void LoadMenu_Click(object sender, RoutedEventArgs e) => LoadFromFile();

        private void AddButton_Click(object sender, RoutedEventArgs e) => AddNewSchedule();

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string destination = destinationTextBox.Text;
            if (DateTime.TryParse(filterDateTextBox.Text, out DateTime beforeArrival))
            {
                FilterSchedules(destination, beforeArrival);
            }
            else
            {
                MessageBox.Show("Некорректная дата");
            }
        }


        private void Perehod_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        
    }
}
