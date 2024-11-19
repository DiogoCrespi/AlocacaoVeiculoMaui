using Microsoft.Maui.Controls;

namespace AlocacaoVeiuculo.Pages
{
    public partial class FuncionarioDashboard : ContentPage
    {
        public FuncionarioDashboard()
        {
            InitializeComponent();
        }

        private async void OnGerarRelatoriosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Relatórios", "Opção de relatórios ainda será implementada.", "OK");
        }

        private async void OnGerenciarVeiculosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Veículos", "Opção de gerenciamento de veículos ainda será implementada.", "OK");
        }

        private async void OnGerenciarReservasClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reservas", "Opção de gerenciamento de reservas ainda será implementada.", "OK");
        }
    }
}
