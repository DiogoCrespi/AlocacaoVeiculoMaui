using AlocacaoVeiuculo.Data;
using AlocacaoVeiuculo.Modelo;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Pages
{
    public partial class UsuarioReservas : ContentPage
    {
        private Usuario usuario;
        private ReservaData reservaData;
        public ObservableCollection<Reserva> Reservas { get; set; }

        public UsuarioReservas(Usuario usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            reservaData = new ReservaData();
            Reservas = new ObservableCollection<Reserva>();
            ReservasCollectionView.ItemsSource = Reservas;
            CarregarReservas();
        }

        private async void CarregarReservas()
        {
            try
            {
                Reservas.Clear();
                var reservas = await reservaData.ObterReservasPorUsuarioAsync(usuario.Id);
                foreach (var reserva in reservas)
                {
                    Reservas.Add(reserva);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar reservas: {ex.Message}", "OK");
            }
        }

        private void OnMostrarDadosUsuarioClicked(object sender, EventArgs e)
        {
            // Toggle the visibility of the user data panel
            UsuarioDadosPanel.IsVisible = !UsuarioDadosPanel.IsVisible;

            if (UsuarioDadosPanel.IsVisible)
            {
                // Update UI with user data when panel is shown
                UsuarioNome.Text = $"Nome: {usuario.Nome}";
                UsuarioSenha.Text = $"Senha: {usuario.Senha}";
                UsuarioCpf.Text = $"CPF: {usuario.Cpf}";
                UsuarioDataNascimento.Text = $"Data de Nascimento: {usuario.DataNascimento.ToShortDateString()}";
                UsuarioTelefone.Text = $"Telefone: {usuario.Telefone}";
            }
        }

        private async void OnMostrarAlugueisClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Meus Aluguéis", "Aqui você verá seus aluguéis.", "OK");
        }

        private async void OnSolicitarAluguelClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Solicitar Aluguel", "Aqui você poderá solicitar um novo aluguel.", "OK");
        }
    }
}
