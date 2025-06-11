using System;
using System.Collections.Generic;
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
using static WpfApp15.MainWindow;

namespace WpfApp15
{
    /// <summary>
    /// Логика взаимодействия для ScheduleEditWindow.xaml
    /// </summary>
    public partial class ScheduleEditWindow : Window
    {
        public BusSchedule BusSchedule { get; private set; }

        public ScheduleEditWindow()
        {
            InitializeComponent();

            DepartureDatePicker.SelectedDate = DateTime.Now;
            ArrivalDatePicker.SelectedDate = DateTime.Now.AddHours(1);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(BusNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(BusTypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(DestinationTextBox.Text) ||
                !DepartureDatePicker.SelectedDate.HasValue ||
                !ArrivalDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, заполните все поля");
                return;
            }

            BusSchedule = new BusSchedule
            {
                BusNumber = BusNumberTextBox.Text,
                BusType = BusTypeTextBox.Text,
                Destination = DestinationTextBox.Text,
                DepartureDateTime = DepartureDatePicker.SelectedDate.Value,
                ArrivalDateTime = ArrivalDatePicker.SelectedDate.Value
            };

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
