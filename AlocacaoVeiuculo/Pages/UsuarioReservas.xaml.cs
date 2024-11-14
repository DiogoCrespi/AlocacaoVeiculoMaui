using AlocacaoVeiuculo.Data;
using AlocacaoVeiuculo.Modelo;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AlocacaoVeiuculo.Pages
{
    public partial class UsuarioReservas : ContentPage
    {
        private Usuario usuario;
        private ReservaData reservaData;
        private DisponibilidadeData disponibilidadeData;
        public ObservableCollection<Reserva> Reservas { get; set; }

        public UsuarioReservas(Usuario usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            reservaData = new ReservaData();
            disponibilidadeData = new DisponibilidadeData();
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
            UsuarioDadosPanel.IsVisible = !UsuarioDadosPanel.IsVisible;
            AlugarVeiculoPanel.IsVisible = false;

            if (UsuarioDadosPanel.IsVisible)
            {
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
            AlugarVeiculoPanel.IsVisible = !AlugarVeiculoPanel.IsVisible;
            UsuarioDadosPanel.IsVisible = false;

            if (AlugarVeiculoPanel.IsVisible)
            {
                var veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(DateTime.Now, TimeSpan.Zero, DateTime.Now.AddDays(1), TimeSpan.Zero);
                VeiculosPicker.ItemsSource = veiculosDisponiveis.Select(v => v.VeiculoId.ToString()).ToList();
            }
        }

        private async void OnFinalizarAlocacaoClicked(object sender, EventArgs e)
        {
            if (VeiculosPicker.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Selecione um veículo para alugar.", "OK");
                return;
            }

            var reserva = new Reserva
            {
                LocalRetirada = entryLocalRetirada.Text,
                DataRetirada = datePickerRetirada.Date,
                HoraRetirada = timePickerRetirada.Time,
                DataDevolucao = datePickerDevolucao.Date,
                HoraDevolucao = timePickerDevolucao.Time,
                UsuarioId = usuario.Id,
                VeiculoId = int.Parse(VeiculosPicker.SelectedItem.ToString()),
                VeiculoTipo = "Carro" // ou "Moto", de acordo com a escolha do veículo
            };

            await reservaData.AdicionarReservaAsync(reserva);
            await DisplayAlert("Reserva Confirmada", "Seu aluguel foi registrado com sucesso!", "OK");
            CarregarReservas();
            AlugarVeiculoPanel.IsVisible = false;
        }
    }
}
