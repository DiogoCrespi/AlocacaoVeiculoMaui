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
            await DisplayAlert("Relat�rios", "Op��o de relat�rios ainda ser� implementada.", "OK");
        }

        private async void OnGerenciarVeiculosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Ve�culos", "Op��o de gerenciamento de ve�culos ainda ser� implementada.", "OK");
        }

        private async void OnGerenciarReservasClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reservas", "Op��o de gerenciamento de reservas ainda ser� implementada.", "OK");
        }
    }
}
