using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient = new HttpClient();
        private const string apiUrl = "http://localhost:8000/users/";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetUsersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var users = await GetUsersAsync();
                UsersListBox.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<User>> GetUsersAsync()
        {
            var response = await httpClient.GetStringAsync(apiUrl);
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        private async void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                var user = new User
                {
                    Name = UserNameTextBox.Text,
                    Email = UserEmailTextBox.Text,
                    Age = int.Parse(UserAgeTextBox.Text)
                };

                try
                {
                    await AddUserAsync(user);
                    MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task AddUserAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            MessageBox.Show($"Sending JSON: {json}");

            var response = await httpClient.PostAsync(apiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Server response: {response.StatusCode}, Details: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error adding user: {response.StatusCode} - {responseContent}");
            }

            response.EnsureSuccessStatusCode();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(UserNameTextBox.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(UserEmailTextBox.Text) || !IsValidEmail(UserEmailTextBox.Text))
            {
                MessageBox.Show("Valid email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(UserAgeTextBox.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Age must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
