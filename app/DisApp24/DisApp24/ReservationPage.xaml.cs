using System;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using PhoneNumbers;

namespace DisApp24
{
    public partial class ReservationPage : ContentPage
    {
        public ReservationPage()
        {
            InitializeComponent();
            InitializePickers();
            WeakReferenceMessenger.Default.Register<SelectedDateMessage>(this, (recipient, message) =>
            {
                SelectedDateLabel.Text = $"Ausgewähltes Datum: {message.Date.ToString("dd.MM.yyyy")}";
            });
        }

        private void InitializePickers()
        {
            // Populate the ResourcePicker with sample data
            ResourcePicker.ItemsSource = new string[]
            {
                "Ressource 1",
                "Ressource 2",
                "Ressource 3"
            };

            // Populate the TimePicker with time slots
            TimePicker.ItemsSource = new string[]
            {
                "08:00-09:00",
                "09:00-10:00",
                "10:00-11:00",
                "11:00-12:00",
                "12:00-13:00",
                "13:00-14:00",
                "14:00-15:00",
                "15:00-16:00"
            };
        }

        //Input-Validation 
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var parsedNumber = phoneUtil.Parse(phoneNumber, null);
                return phoneUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
            {
                FirstNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Vornamen ein.");
                isValid = false;
            }
            else
            {
                FirstNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
            {
                LastNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Nachnamen ein.");
                isValid = false;
            }
            else
            {
                LastNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || !IsValidEmail(EmailEntry.Text))
            {
                EmailFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
                isValid = false;
            }
            else
            {
                EmailFrame.BorderColor = Colors.DimGray;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumberEntry.Text) && !IsValidPhoneNumber(PhoneNumberEntry.Text))
            {
                PhoneNumberFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige Telefonnummer ein.");
                isValid = false;
            }
            else
            {
                PhoneNumberFrame.BorderColor = Colors.DimGray;
            }

            if (ResourcePicker.SelectedIndex == -1)
            {
                ResourceFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte wählen Sie eine Ressource aus.");
                isValid = false;
            }
            else
            {
                ResourceFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(SelectedDateLabel.Text) || SelectedDateLabel.Text == "Datum auswählen")
            {
                SelectedDateLabel.TextColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte wählen Sie ein Datum aus.");
                isValid = false;
            }
            else
            {
                SelectedDateLabel.TextColor = Colors.Black;
            }

            if (TimePicker.SelectedIndex == -1)
            {
                TimeFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte wählen Sie eine Uhrzeit aus.");
                isValid = false;
            }
            else
            {
                TimeFrame.BorderColor = Colors.DimGray;
            }

            ValidationLabel.Text = string.Join("\n", errorMessages);

            return isValid;
        }



        private async void OnSelectDateButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CalendarPage());
        }

        private void OnReserveButtonClicked(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                // Code zur Implementierung der Reservierungsfunktionalität
            }
        }
    }
}