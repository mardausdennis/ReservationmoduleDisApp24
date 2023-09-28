using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Helpers;
using DisApp24.Services;
using DisApp24.Models;
using DisApp24.Resources;
using Microsoft.Maui.Controls;


namespace DisApp24.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _password;
        private string _confirmPassword;
        private string _validationMessage;

        public RegistrationViewModel()
        {
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();
            RegisterCommand = new Command(async () => await OnRegisterButtonClicked());
            NavigateToLoginCommand = new Command(async () => await OnCancelButtonClicked());
        }

        // ICommand properties for handling button clicks
        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);
        }


        private async Task OnRegisterButtonClicked()
        {
            if (!ValidateInput())
            {
                return;
            }

            if (Password != ConfirmPassword)
            {
                ValidationMessage = AppResources.PasswordsDoNotMatch;
                return;
            }

            try
            {
                var result = await _firebaseAuthService.SignUpWithEmailPasswordAsync(Email, Password, FirstName, LastName, PhoneNumber);
                WeakReferenceMessenger.Default.Send(new RegistrationMessage(true));
                WeakReferenceMessenger.Default.Send(new UserSignedInMessage());
                await Shell.Current.DisplayAlert(AppResources.SuccessTitle, AppResources.RegistrationSuccessMessage, "OK");
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error: {ex.Message}"; 
            }
        }

        private async Task OnCancelButtonClicked()
        {
            WeakReferenceMessenger.Default.Send(new RegistrationMessage(false));
            await AppShell.Current.GoToAsync(nameof(LoginPage));
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                errorMessages.Add(AppResources.EnterFirstName);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                errorMessages.Add(AppResources.EnterLastName);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email) || !InputValidationHelper.IsValidEmail(Email))
            {
                errorMessages.Add(AppResources.EnterValidEmail);
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber) && !InputValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                errorMessages.Add(AppResources.EnterValidPhoneNumber);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                errorMessages.Add(AppResources.EnterPassword);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword) || ConfirmPassword != Password)
            {
                errorMessages.Add(AppResources.PasswordsDoNotMatch);
                isValid = false;
            }

            ValidationMessage = string.Join("\n", errorMessages);

            return isValid;
        }

    }
}
