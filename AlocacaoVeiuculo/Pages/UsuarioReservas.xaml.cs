using AlocacaoVeiuculo.Data;
using AlocacaoVeiuculo.Modelo;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AlocacaoVeiuculo.Pages
{
    public partial class UsuarioReservas : ContentPage
    {
        private Usuario usuario;
        private ReservaData reservaData;
        private DisponibilidadeData disponibilidadeData;
        public ObservableCollection<Reserva> Reservas { get; set; }
        private string localRetirada;
        private DateTime dataRetirada;
        private TimeSpan horaRetirada;
        private DateTime dataDevolucao;
        private TimeSpan horaDevolucao;

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

        public UsuarioReservas(Usuario usuario, string localRetirada, DateTime dataRetirada, TimeSpan horaRetirada, DateTime dataDevolucao, TimeSpan horaDevolucao, List<Disponibilidade> veiculosDisponiveis)
            : this(usuario)
        {
            this.localRetirada = localRetirada;
            this.dataRetirada = dataRetirada;
            this.horaRetirada = horaRetirada;
            this.dataDevolucao = dataDevolucao;
            this.horaDevolucao = horaDevolucao;

            entryLocalRetirada.Text = localRetirada;
            datePickerRetirada.Date = dataRetirada;
            timePickerRetirada.Time = horaRetirada;
            datePickerDevolucao.Date = dataDevolucao;
            timePickerDevolucao.Time = horaDevolucao;

            VeiculosPicker.ItemsSource = veiculosDisponiveis.Select(v =>
                v.TipoVeiculo == "Carro" ? $"Carro {v.VeiculoId}" : $"Moto {v.VeiculoId}"
            ).ToList();

            AlugarVeiculoPanel.IsVisible = true;
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
            ReservasPanel.IsVisible = false;

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
            ReservasPanel.IsVisible = !ReservasPanel.IsVisible;
            UsuarioDadosPanel.IsVisible = false;
            AlugarVeiculoPanel.IsVisible = false;

            if (ReservasPanel.IsVisible)
            {
                try
                {
                    ReservasPanelContent.Children.Clear();

                    var reservas = await reservaData.ObterReservasPorUsuarioAsync(usuario.Id);

                    if (reservas.Any())
                    {
                        foreach (var reserva in reservas)
                        {
                            ReservasPanelContent.Children.Add(new Label
                            {
                                Text = $"Veículo: {reserva.VeiculoTipo} ID {reserva.VeiculoId}\n" +
                                       $"Local Retirada: {reserva.LocalRetirada}\n" +
                                       $"Data Retirada: {reserva.DataRetirada:dd/MM/yyyy} às {reserva.HoraRetirada}\n" +
                                       $"Data Devolução: {reserva.DataDevolucao:dd/MM/yyyy} às {reserva.HoraDevolucao}",
                                TextColor = Colors.White,
                                FontSize = 14,
                                Margin = new Thickness(0, 5)
                            });
                        }
                    }
                    else
                    {
                        ReservasPanelContent.Children.Add(new Label
                        {
                            Text = "Nenhuma reserva encontrada.",
                            TextColor = Colors.Gray,
                            FontSize = 16,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        });
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Falha ao carregar reservas: {ex.Message}", "OK");
                }
            }
        }

        private async void OnSolicitarAluguelClicked(object sender, EventArgs e)
        {
            AlugarVeiculoPanel.IsVisible = !AlugarVeiculoPanel.IsVisible;
            UsuarioDadosPanel.IsVisible = false;
            ReservasPanel.IsVisible = false;

            if (AlugarVeiculoPanel.IsVisible)
            {
                var veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(DateTime.Now, TimeSpan.Zero, DateTime.Now.AddDays(1), TimeSpan.Zero);
                if (veiculosDisponiveis.Any())
                {
                    VeiculosPicker.ItemsSource = veiculosDisponiveis.Select(v =>
                        v.TipoVeiculo == "Carro" ? $"Carro {v.VeiculoId}" : $"Moto {v.VeiculoId}"
                    ).ToList();
                }
                else
                {
                    VeiculosPicker.ItemsSource = new List<string> { "Nenhum veículo cadastrado" };
                }
            }
        }

        private async void OnFinalizarAlocacaoClicked(object sender, EventArgs e)
        {
            if (VeiculosPicker.SelectedItem == null || VeiculosPicker.SelectedItem.ToString() == "Nenhum veículo cadastrado")
            {
                await DisplayAlert("Erro", "Nenhum veículo selecionado para alugar.", "OK");
                return;
            }

            var veiculoSelecionado = VeiculosPicker.SelectedItem.ToString();
            var veiculoId = int.Parse(veiculoSelecionado.Split(' ')[1]);
            var veiculoTipo = veiculoSelecionado.Split(' ')[0];

            var reserva = new Reserva
            {
                LocalRetirada = entryLocalRetirada.Text,
                DataRetirada = datePickerRetirada.Date,
                HoraRetirada = timePickerRetirada.Time,
                DataDevolucao = datePickerDevolucao.Date,
                HoraDevolucao = timePickerDevolucao.Time,
                UsuarioId = usuario.Id,
                VeiculoId = veiculoId,
                VeiculoTipo = veiculoTipo
            };

            await reservaData.AdicionarReservaAsync(reserva);
            await DisplayAlert("Reserva Confirmada", "Seu aluguel foi registrado com sucesso!", "OK");
            CarregarReservas();
            AlugarVeiculoPanel.IsVisible = false;
        }
    }
}
