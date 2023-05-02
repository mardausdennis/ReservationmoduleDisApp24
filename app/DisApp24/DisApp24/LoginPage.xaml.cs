using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DisApp24.ViewModels;

namespace DisApp24
{
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {
            InitializeComponent();

            var viewModel = ServiceHelper.GetService<LoginPageViewModel>();
            BindingContext = viewModel;
            viewModel.Initialize(Navigation);
        }

        protected override bool OnBackButtonPressed()
        {
            // Call the ViewModel's OnBackButtonPressedAsync
            if (BindingContext is LoginPageViewModel viewModel)
            {
                viewModel.BackButtonCommand.Execute(null);
            }

            // Return true to prevent the default processing of the back button
            return true;
        }


    }
}
