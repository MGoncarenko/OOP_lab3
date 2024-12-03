using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace oopLab3
{
    public partial class MainPage : ContentPage
    {
        private int nextId = 1;

        public ObservableCollection<Person> People { get; set; } = new();
        public ObservableCollection<Person> FilteredPeople { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            UpdateFilteredPeople();
        }

        private async void LoadJsonFile(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync();
                if (fileResult != null && fileResult.FileName.EndsWith(".json"))
                {
                    var json = File.ReadAllText(fileResult.FullPath);
                    var people = JsonSerializer.Deserialize<ObservableCollection<Person>>(json);
                    if (people != null)
                    {
                        People.Clear();
                        foreach (var person in people)
                        {
                            People.Add(person);
                            nextId = Math.Max(nextId, person.Id + 1);
                        }
                        UpdateFilteredPeople();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Не вдалося завантажити файл: {ex.Message}", "OK");
            }
        }

        private async void SaveJsonFile(object sender, EventArgs e)
        {
            try
            {
                var json = JsonSerializer.Serialize(People, new JsonSerializerOptions { WriteIndented = true });
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = Path.Combine(desktopPath, "output.json");
                File.WriteAllText(fileName, json);
                await DisplayAlert("Успіх", $"Файл збережено на робочому столі: {fileName}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Не вдалося зберегти файл: {ex.Message}", "OK");
            }
        }

        private async void EditPerson(object sender, EventArgs e)
        {
            if ((sender as Button)?.BindingContext is Person selectedPerson)
            {
                var editPage = new EditPersonPage(selectedPerson);
                await Navigation.PushAsync(editPage);
            }
        }

        private void AddPerson(object sender, EventArgs e)
        {
            var newPerson = new Person
            {
                Id = nextId++,
                Name = "Новий",
                Age = 0,
                Faculty = "Unknown"
            };

            People.Add(newPerson);
            UpdateFilteredPeople();
        }

        private void DeletePerson(object sender, EventArgs e)
        {
            if ((sender as Button)?.BindingContext is Person selectedPerson)
            {
                People.Remove(selectedPerson);
                UpdateFilteredPeople();
            }
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            var query = e.NewTextValue?.ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(query))
            {
                UpdateFilteredPeople();
            }
            else
            {
                var filtered = People.Where(p =>
                    p.Id.ToString().Contains(query) ||
                    p.Name.ToLower().Contains(query) ||
                    p.Age.ToString().Contains(query) ||
                    p.Faculty.ToLower().Contains(query)).ToList();

                FilteredPeople.Clear();
                foreach (var person in filtered)
                {
                    FilteredPeople.Add(person);
                }
            }
        }

        private void UpdateFilteredPeople()
        {
            FilteredPeople.Clear();
            foreach (var person in People)
            {
                FilteredPeople.Add(person);
            }
        }
    }
}
