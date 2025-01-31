using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagementWPF
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7048/api/")
            };
        }

        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleSelector.SelectedItem == null)
            {
                MessageBox.Show("Please select a role!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedRole = (RoleSelector.SelectedItem as ComboBoxItem).Content.ToString();
            ConfigureTabsBasedOnRole(selectedRole);
        }

        private void ConfigureTabsBasedOnRole(string role)
        {
            MainTabControl.Visibility = Visibility.Visible;

            if (role == "Partner")
            {
                RegisterPartnerTab.Visibility = Visibility.Visible;
                CreateEventTab.Visibility = Visibility.Visible;
                SetTicketsTab.Visibility = Visibility.Visible;
                SellTicketsTab.Visibility = Visibility.Visible;
                PartnerSalesTab.Visibility = Visibility.Visible;

                AdminSalesTab.Visibility = Visibility.Collapsed;
            }
            else if (role == "Admin")
            {
                AdminSalesTab.Visibility = Visibility.Visible;

                RegisterPartnerTab.Visibility = Visibility.Collapsed;
                CreateEventTab.Visibility = Visibility.Collapsed;
                SetTicketsTab.Visibility = Visibility.Collapsed;
                SellTicketsTab.Visibility = Visibility.Collapsed;
                PartnerSalesTab.Visibility = Visibility.Collapsed;
            }
        }

        private async void RegisterPartner_Click(object sender, RoutedEventArgs e)
        {
            var partner = new { Name = PartnerName.Text, Email = PartnerEmail.Text, Phone = PartnerPhone.Text };
            var response = await client.PostAsJsonAsync("Partners/register", partner);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var createdPartner = JsonSerializer.Deserialize<Partner>(responseData);

                MessageBox.Show($"Partner registered successfully! Your Partner ID is {createdPartner.PartnerId}. Please use this Partner ID to create new events and view your ticket sales.");
            }
            else
            {
                MessageBox.Show($"Error occurred: {response.ReasonPhrase}");
            }
        }
        public class Partner
        {
            [JsonPropertyName("partnerId")]
            public int PartnerId { get; set; }
        }

        private async void CreateEvent_Click(object sender, RoutedEventArgs e)
        {
            var newEvent = new
            {
                Name = EventName.Text,
                Location = EventLocation.Text,
                Date = EventDate.SelectedDate.Value.ToString("yyyy-MM-dd"),
                TicketPrice = int.Parse(EventTicketPrice.Text),
                CommissionRate=int.Parse(EventCommissionRate.Text),
                PartnerId = int.Parse(EventPartnerId.Text)
            };
            var response = await client.PostAsJsonAsync("Events/create", newEvent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var createdEvent = JsonSerializer.Deserialize<Event>(responseData);

                MessageBox.Show($"Event created successfully! Your Event ID is {createdEvent.EventId}. Please use this Event ID to set ticket details and sell tickets.");
            }
            else
            {
                MessageBox.Show($"Error occurred: {response.ReasonPhrase}");
            }
        }
        public class Event
        {
            [JsonPropertyName("eventId")]
            public int EventId { get; set; }
        }

        private async void SetTicketDetails_Click(object sender, RoutedEventArgs e)
        {
            var response = await client.PostAsJsonAsync($"Events/{SetTicketEventId.Text}/set-tickets", int.Parse(SetTicketTotalTickets.Text));
            MessageBox.Show(response.IsSuccessStatusCode ? "Ticket details updated successfully!" : "Error occurred!");
        }

        private async void SellTickets_Click(object sender, RoutedEventArgs e)
        {
            var response = await client.PostAsJsonAsync($"Events/{SellTicketEventId.Text}/sell-tickets", int.Parse(SellTicketCount.Text));
            MessageBox.Show(response.IsSuccessStatusCode ? "Tickets sold successfully!" : "Error occurred!");
        }

        private async void ViewPartnerSales_Click(object sender, RoutedEventArgs e)
        {
            var partnerId = PartnerSalesPartnerId.Text;

            var response = await client.GetAsync($"Events/partner/{partnerId}/sales-status");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var partnerSales = JsonSerializer.Deserialize<List<PartnerSale>>(result);

                PartnerSalesDataGrid.ItemsSource = partnerSales;
            }
            else
            {
                MessageBox.Show("Error occurred!");
            }
        }

        public class PartnerSale
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("ticketsSold")]
            public int TicketsSold { get; set; }

            [JsonPropertyName("totalTickets")]
            public int TotalTickets { get; set; }
        }
        private async void ViewAdminSales_Click(object sender, RoutedEventArgs e)
        {
            var response = await client.GetAsync("Events/admin/sales-status");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var adminSales = JsonSerializer.Deserialize<List<AdminSale>>(result);

                AdminSalesDataGrid.ItemsSource = adminSales;
            }
            else
            {
                MessageBox.Show("Error occurred!");
            }
        }
        public class AdminSale
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("ticketsSold")]
            public int TicketsSold { get; set; }

            [JsonPropertyName("totalTickets")]
            public int TotalTickets { get; set; }

            [JsonPropertyName("commissionEarned")]
            public string CommissionEarned { get; set; }
        }

    }
}
    