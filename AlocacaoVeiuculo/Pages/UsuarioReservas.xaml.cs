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

        private List<Disponibilidade> veiculosDisponiveis;
        private string tipoVeiculoSelecionado;

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
            this.veiculosDisponiveis = veiculosDisponiveis;

            entryLocalRetirada.Text = localRetirada;
            datePickerRetirada.Date = dataRetirada;
            timePickerRetirada.Time = horaRetirada;
            datePickerDevolucao.Date = dataDevolucao;
            timePickerDevolucao.Time = horaDevolucao;

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
                veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(DateTime.Now, TimeSpan.Zero, DateTime.Now.AddDays(1), TimeSpan.Zero);
                GerarCaixasVeiculos();
            }
        }

        private void OnSelecionarCarrosClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Carro";
            GerarCaixasVeiculos();
        }

        private void OnSelecionarMotosClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Moto";
            GerarCaixasVeiculos();
        }

        private void GerarCaixasVeiculos()
        {
            if (veiculosDisponiveis == null) return;

            var veiculosFiltrados = veiculosDisponiveis
                .Where(v => v.TipoVeiculo.Equals(tipoVeiculoSelecionado, StringComparison.OrdinalIgnoreCase))
                .ToList();

            GridCaixasVeiculos.Children.Clear();

            if (veiculosFiltrados.Any())
            {
                FrameCaixasVeiculos.IsVisible = true;

                int colunas = 4; // Define o número de colunas
                GridCaixasVeiculos.ColumnDefinitions.Clear();
                for (int i = 0; i < colunas; i++)
                {
                    GridCaixasVeiculos.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                GridCaixasVeiculos.RowDefinitions.Clear();
                for (int i = 0; i < (veiculosFiltrados.Count + colunas - 1) / colunas; i++)
                {
                    GridCaixasVeiculos.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                int linha = 0, coluna = 0;

                foreach (var veiculo in veiculosFiltrados)
                {
                    var stackLayout = new StackLayout
                    {
                        Children =
                {
                    new Image
                    {
                        Source = string.IsNullOrEmpty(veiculo.ImagemPath) ? "placeholder.png" : ImageSource.FromFile(veiculo.ImagemPath),
                        HeightRequest = 100,
                        WidthRequest = 100,
                        Aspect = Aspect.AspectFill
                    },
                    new Label { Text = $"ID: {veiculo.VeiculoId}", TextColor = Colors.White, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"Tipo: {veiculo.TipoVeiculo}", TextColor = Colors.White },
                    new Label { Text = "Disponível", TextColor = Colors.LightGreen }
                }
                    };

                    var frame = new Frame
                    {
                        BackgroundColor = Colors.DarkGray,
                        CornerRadius = 10,
                        Padding = 10,
                        Margin = new Thickness(5),
                        Content = stackLayout
                    };

                    Grid.SetRow(frame, linha);
                    Grid.SetColumn(frame, coluna);
                    GridCaixasVeiculos.Children.Add(frame);

                    coluna++;
                    if (coluna >= colunas)
                    {
                        coluna = 0;
                        linha++;
                    }
                }
            }
            else
            {
                FrameCaixasVeiculos.IsVisible = false;
                DisplayAlert("Aviso", "Nenhum veículo disponível.", "OK");
            }
        }

        private async void OnFinalizarAlocacaoClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tipoVeiculoSelecionado))
            {
                await DisplayAlert("Erro", "Selecione um tipo de veículo antes de finalizar.", "OK");
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
                VeiculoId = 0, // Lógica para buscar o veículo selecionado
                VeiculoTipo = tipoVeiculoSelecionado
            };

            await reservaData.AdicionarReservaAsync(reserva);
            await DisplayAlert("Reserva Confirmada", "Seu aluguel foi registrado com sucesso!", "OK");
            CarregarReservas();
            AlugarVeiculoPanel.IsVisible = false;
        }
    }
}
