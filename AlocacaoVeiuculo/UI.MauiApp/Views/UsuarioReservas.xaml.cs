using System.Collections.ObjectModel;

using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.Data.Vehicles;
using AlocacaoVeiuculo.Data.Reservations;
using System.Globalization;
using System.Text.Json;

namespace AlocacaoVeiuculo.Pages
{
    public partial class UsuarioReservas : ContentPage
    {
        private Usuario usuario;
        private ReservaData reservaData;
        private DisponibilidadeData disponibilidadeData;
        private Disponibilidade veiculoSelecionado;
        private Reserva reservaSelecionada;
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
                var reservas = await reservaData.ObterReservasPorUsuarioAsync(usuario.Id);

                Reservas.Clear();
                foreach (var reserva in reservas)
                {
                    if (reserva.VeiculoTipo == "Carro")
                    {
                        var carro = await new CarroData().ObterCarroPorIdAsync(reserva.VeiculoId);
                        if (carro != null)
                        {
                            reserva.ModeloVeiculo = carro.Modelo;
                            reserva.IsDisponivel = !carro.IsAlugado;
                        }
                        else
                        {
                            reserva.ModeloVeiculo = "Modelo não encontrado";
                        }
                    }
                    else if (reserva.VeiculoTipo == "Moto")
                    {
                        var moto = await new MotoData().ObterMotoPorIdAsync(reserva.VeiculoId);
                        if (moto != null)
                        {
                            reserva.ModeloVeiculo = moto.Modelo;
                            reserva.IsDisponivel = !moto.IsAlugado;
                        }
                        else
                        {
                            reserva.ModeloVeiculo = "Modelo não encontrado";
                        }
                    }

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
            FrameCaixasVeiculos.IsVisible = false;

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
            FrameCaixasVeiculos.IsVisible = false;

            if (ReservasPanel.IsVisible)
            {
                try
                {
                    ReservasPanelContent.Children.Clear();

                    var reservas = await reservaData.ObterReservasPorUsuarioAsync(usuario.Id);

                    if (reservas == null || !reservas.Any())
                    {
                        ReservasPanelContent.Children.Add(new Label
                        {
                            Text = "Nenhuma reserva encontrada.",
                            TextColor = Colors.Gray,
                            FontSize = 16,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        });
                        return;
                    }

                    foreach (var reserva in reservas)
                    {
                        string modelo = reserva.VeiculoTipo == "Carro"
                            ? (await new CarroData().ObterCarroPorIdAsync(reserva.VeiculoId))?.Modelo ?? "Modelo não encontrado"
                            : (await new MotoData().ObterMotoPorIdAsync(reserva.VeiculoId))?.Modelo ?? "Modelo não encontrado";

                        var stackLayout = new StackLayout
                        {
                            Margin = new Thickness(0, 10),
                            Children =
                    {
                        new Label
                        {
                            Text = $"{reserva.VeiculoTipo}: {modelo}",
                            TextColor = Colors.White,
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold
                        },
                        new Label
                        {
                            Text = $"Local Retirada: {reserva.LocalRetirada}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Data Retirada: {reserva.DataRetirada:dd/MM/yyyy} às {reserva.HoraRetirada}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Data Devolução: {reserva.DataDevolucao:dd/MM/yyyy} às {reserva.HoraDevolucao}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        }
                    }
                        };

                        if (!reserva.IsDisponivel)
                        {
                            stackLayout.Children.Add(new Label
                            {
                                Text = "Reserva Cancelada",
                                TextColor = Colors.Red,
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold
                            });
                        }
                        else
                        {
                            stackLayout.Children.Add(new Button
                            {
                                Text = "Cancelar Reserva",
                                BackgroundColor = Colors.Red,
                                TextColor = Colors.White,
                                WidthRequest = 150,
                                HeightRequest = 40,  
                                HorizontalOptions = LayoutOptions.Start,
                                Command = new Command(async () => await CancelarReserva(reserva.Id))
                            });
                        }

                        ReservasPanelContent.Children.Add(stackLayout);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Falha ao carregar reservas: {ex.Message}", "OK");
                }
            }
        }
        private async Task CancelarReserva(int reservaId)
        {
            try
            {
                var reserva = await reservaData.ObterReservaPorIdAsync(reservaId);
                if (reserva == null)
                {
                    await DisplayAlert("Erro", "Reserva não encontrada.", "OK");
                    return;
                }

                string motivoExclusao = await DisplayPromptAsync(
                    "Motivo da Exclusão",
                    "Por favor, informe o motivo para cancelar a reserva:",
                    "OK",
                    "Cancelar");

                if (string.IsNullOrWhiteSpace(motivoExclusao))
                {
                    await DisplayAlert("Erro", "O motivo da exclusão é obrigatório.", "OK");
                    return;
                }

                reserva.IsDisponivel = false;
                reserva.MotivoExclusao = motivoExclusao;

                await reservaData.AtualizarReservaAsync(reserva);

                await DisplayAlert("Sucesso", "Reserva cancelada com sucesso.", "OK");

                CarregarReservas();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao cancelar a reserva: {ex.Message}", "OK");
            }
        }










        private async void OnSolicitarAluguelClicked(object sender, EventArgs e)
        {
            AlugarVeiculoPanel.IsVisible = !AlugarVeiculoPanel.IsVisible;
            UsuarioDadosPanel.IsVisible = false;
            ReservasPanel.IsVisible = false;
            FrameCaixasVeiculos.IsVisible = false;

            if (AlugarVeiculoPanel.IsVisible)
            {
                if (veiculosDisponiveis == null || !veiculosDisponiveis.Any())
                {
                    veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(DateTime.Now, TimeSpan.Zero, DateTime.Now.AddDays(1), TimeSpan.Zero);
                }

                GerarCaixasVeiculos();
                FrameCaixasVeiculos.IsVisible = false;
            }
        }
 





















        private async void OnSelecionarCarrosClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Carro";
            await GerarCaixasVeiculos();
        }

        private async void OnSelecionarMotosClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Moto";
            await GerarCaixasVeiculos();
        }

        private async Task GerarCaixasVeiculos()
        {
            if (veiculosDisponiveis == null || !veiculosDisponiveis.Any())
            {
                FrameCaixasVeiculos.IsVisible = false;
                GridCaixasVeiculos.Children.Clear();
                GridCaixasVeiculos.Children.Add(new Label
                {
                    Text = tipoVeiculoSelecionado == "Carro" ? "Nenhum carro disponível" : "Nenhuma moto disponível",
                    TextColor = Colors.White,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                await DisplayAlert("Atenção", "Nenhum veículo disponível no momento.", "OK");
                return;
            }

            var veiculosFiltrados = new List<(Disponibilidade, bool isAlugado)>();

            if (tipoVeiculoSelecionado == "Carro")
            {
                var carroData = new CarroData();
                foreach (var disponibilidade in veiculosDisponiveis.Where(v => v.TipoVeiculo == "Carro"))
                {
                    var carro = await carroData.ObterCarroPorIdAsync(disponibilidade.VeiculoId);
                    if (carro != null && !carro.IsAlugado)
                    {
                        veiculosFiltrados.Add((disponibilidade, carro.IsAlugado));
                    }
                }
            }
            else if (tipoVeiculoSelecionado == "Moto")
            {
                var motoData = new MotoData();
                foreach (var disponibilidade in veiculosDisponiveis.Where(v => v.TipoVeiculo == "Moto"))
                {
                    var moto = await motoData.ObterMotoPorIdAsync(disponibilidade.VeiculoId);
                    if (moto != null && !moto.IsAlugado)
                    {
                        veiculosFiltrados.Add((disponibilidade, moto.IsAlugado));
                    }
                }
            }

            GridCaixasVeiculos.Children.Clear();

            if (veiculosFiltrados.Any())
            {
                FrameCaixasVeiculos.IsVisible = true;
                int colunas = 4;

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

                foreach (var (disponibilidade, isAlugado) in veiculosFiltrados)
                {
                    var stackLayout = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                {
                    new Image
                    {
                        Source = string.IsNullOrEmpty(disponibilidade.ImagemPath) ? "placeholder.png" : ImageSource.FromFile(disponibilidade.ImagemPath),
                        HeightRequest = 120,
                        WidthRequest = 200,
                        Aspect = Aspect.AspectFill
                    },
                    new Label
                    {
                        Text = $"Modelo: {disponibilidade.Modelo}",
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = $"Combustível: {disponibilidade.TipoCombustivel}",
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = $"Disponível: {(!isAlugado ? "Sim" : "Não")}",
                        TextColor = !isAlugado ? Colors.Green : Colors.Red,
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
                    };

                    var frame = new Frame
                    {
                        Content = stackLayout,
                        BackgroundColor = veiculoSelecionado == disponibilidade ? Colors.Gray : Colors.Transparent,
                        BorderColor = veiculoSelecionado == disponibilidade ? Colors.Red : Colors.Gray,
                        CornerRadius = 10,
                        Padding = 5,
                        Margin = new Thickness(5),
                        HasShadow = true
                    };

                    frame.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() => SelecionarVeiculo(disponibilidade))
                    });

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
            }
        }

        private void SelecionarVeiculo(Disponibilidade veiculo)
        {
            veiculoSelecionado = veiculo;
            GerarCaixasVeiculos();
        }

        private async void OnFinalizarAlocacaoClicked(object sender, EventArgs e)
        {
            FrameCaixasVeiculos.IsVisible = false;
            if (veiculoSelecionado == null)
            {
                await DisplayAlert("Erro", "Selecione um veículo antes de finalizar.", "OK");
                return;
            }
            if (datePickerRetirada.Date.DayOfWeek == DayOfWeek.Saturday || datePickerRetirada.Date.DayOfWeek == DayOfWeek.Sunday ||
     datePickerDevolucao.Date.DayOfWeek == DayOfWeek.Saturday || datePickerDevolucao.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                await DisplayAlert("Erro", "Retirada ou devolução não podem ser feitas em finais de semana.", "OK");
                return;
            }

            if (datePickerRetirada.Date >= datePickerDevolucao.Date)
            {
                await DisplayAlert("Erro", "A data de retirada deve ser anterior à data de devolução.", "OK");
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
                VeiculoId = veiculoSelecionado.VeiculoId,
                VeiculoTipo = veiculoSelecionado.TipoVeiculo
            };

            await reservaData.AdicionarReservaAsync(reserva);

            if (veiculoSelecionado.TipoVeiculo == "Carro")
            {
                var carroData = new CarroData();
                var carro = await carroData.ObterCarroPorIdAsync(veiculoSelecionado.VeiculoId);
                if (carro != null)
                {
                    carro.IsAlugado = true;
                    await carroData.AtualizarCarroAsync(carro);
                }
            }
            else if (veiculoSelecionado.TipoVeiculo == "Moto")
            {
                var motoData = new MotoData();
                var moto = await motoData.ObterMotoPorIdAsync(veiculoSelecionado.VeiculoId);
                if (moto != null)
                {
                    moto.IsAlugado = true;
                    await motoData.AtualizarMotoAsync(moto);
                }
            }

            await DisplayAlert("Reserva Confirmada", "Seu aluguel foi registrado com sucesso!", "OK");
            CarregarReservas();
            AlugarVeiculoPanel.IsVisible = false;
        }
    }
}
