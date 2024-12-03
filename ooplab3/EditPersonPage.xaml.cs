using Microsoft.Maui.Controls;

namespace oopLab3
{
    public partial class EditPersonPage : ContentPage
    {
        public Person CurrentPerson { get; }

        public EditPersonPage(Person person)
        {
            InitializeComponent();
            CurrentPerson = person;
            BindingContext = CurrentPerson;
        }

        private async void SaveChanges(object sender, EventArgs e)
        {
            // Повернення на попередню сторінку
            await Navigation.PopAsync();
        }
    }
}
