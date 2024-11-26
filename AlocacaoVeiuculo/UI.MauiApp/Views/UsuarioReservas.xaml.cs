
using System.Collections.ObjectModel;

using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.Data.Vehicles;
using AlocacaoVeiuculo.Data.Reservations;

namespace AlocacaoVeiuculo.Pages
{
    public partial class UsuarioReservas : ContentPage
    {
        private Usuario usuario;
        private ReservaData reservaData;
        private DisponibilidadeData disponibilidadeData;
        private Reserva reservaSelecionada;
        private Disponibilidade veiculoSelecionado;
        private bool exibirReservasCanceladas = true;
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
                            reserva.IsDisponivel = !carro.IsAlugado; // Baseado em IsAlugado
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
                            reserva.IsDisponivel = !moto.IsAlugado; // Baseado em IsAlugado
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



        //--------------------------------------------
        //serve apenas para ocultar os que ja estao Excluidos preguica de mudar todo o codigo
        private async void CarregarReservasNovo()
        {
            try
            {
                var reservas = await reservaData.ObterReservasPorUsuarioAsync(usuario.Id);
                ReservasPanelContent.Children.Clear();

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

                
                var reservasFiltradas = exibirReservasCanceladas
                    ? reservas
                    : reservas.Where(r => r.IsDisponivel).ToList();

                if (!reservasFiltradas.Any())
                {
                    ReservasPanelContent.Children.Add(new Label
                    {
                        Text = "Nenhuma reserva disponível no momento.",
                        TextColor = Colors.Gray,
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    });
                    return;
                }

                foreach (var reserva in reservasFiltradas)
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
                            Text = $"Sua Reserva foi excluída pelo administrador: {reserva.MotivoExclusao}",
                            TextColor = Colors.Red,
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold
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

        //--------------------------------------------


        private void OnMostrarDadosUsuarioClicked(object sender, EventArgs e)
        {
            HeaderToggleCanceladasButton.IsVisible = false;
            UsuarioDadosPanel.IsVisible = !UsuarioDadosPanel.IsVisible;
            AlugarVeiculoPanel.IsVisible = false;
            ReservasPanel.IsVisible = false;
            FrameCaixasVeiculos.IsVisible = false;
            FrameCancelarReservas.IsVisible = false;

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
            HeaderToggleCanceladasButton.IsVisible = true;
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

                    var reservasFiltradas = exibirReservasCanceladas
                        ? reservas
                        : reservas.Where(r => r.IsDisponivel).ToList();

                    if (!reservasFiltradas.Any())
                    {
                        ReservasPanelContent.Children.Add(new Label
                        {
                            Text = "Nenhuma reserva disponível no momento.",
                            TextColor = Colors.Gray,
                            FontSize = 16,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        });
                        return;
                    }

                    foreach (var reserva in reservasFiltradas)
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
                                Text = $"Sua Reserva foi excluída pelo administrador pois: {reserva.MotivoExclusao}",
                                TextColor = Colors.Red,
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold
                            });
                        }
                        else if (!string.IsNullOrWhiteSpace(reserva.MotivoModificacao))
                        {
                            stackLayout.Children.Add(new Label
                            {
                                Text = $"Sua Reserva foi Alterada pelo administrador pois: {reserva.MotivoModificacao}",
                                TextColor = Colors.Yellow,
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold
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

        private async void OnSolicitarAluguelClicked(object sender, EventArgs e)
        {
            // Alterna a visibilidade do painel "Alugar Veículo"
            AlugarVeiculoPanel.IsVisible = !AlugarVeiculoPanel.IsVisible;
            UsuarioDadosPanel.IsVisible = false;
            ReservasPanel.IsVisible = false;
            FrameCaixasVeiculos.IsVisible = false;
            HeaderToggleCanceladasButton.IsVisible = false;
            FrameCancelarReservas.IsVisible = false;

            // Só tenta carregar os veículos disponíveis se o painel estiver visível e ainda não houver veículos carregados
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
             //   await DisplayAlert("Atenção", "Nenhum veículo disponível no momento.", "OK");
            }
        }

        private void SelecionarVeiculo(Disponibilidade veiculo)
        {
            veiculoSelecionado = veiculo;
            GerarCaixasVeiculos(); // Atualiza a interface para refletir a seleção
        }

        private async void OnFinalizarAlocacaoClicked(object sender, EventArgs e)
        {
            FrameCaixasVeiculos.IsVisible = false;
            if (veiculoSelecionado == null)
            {
                await DisplayAlert("Erro", "Selecione um veículo antes de finalizar.", "OK");
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
                    carro.IsAlugado = true; // Marca o carro como alugado
                    await carroData.AtualizarCarroAsync(carro);
                }
            }
            else if (veiculoSelecionado.TipoVeiculo == "Moto")
            {
                var motoData = new MotoData();
                var moto = await motoData.ObterMotoPorIdAsync(veiculoSelecionado.VeiculoId);
                if (moto != null)
                {
                    moto.IsAlugado = true; // Marca a moto como alugada
                    await motoData.AtualizarMotoAsync(moto);
                }
            }

            await DisplayAlert("Reserva Confirmada", "Seu aluguel foi registrado com sucesso!", "OK");
            CarregarReservas();
            AlugarVeiculoPanel.IsVisible = false;
        }



        //--------------------------------------------CancelarReserva------------------------------------

        private void OnCancelarReservaClicked(object sender, EventArgs e)
        {
            // Ocultar outros painéis
            UsuarioDadosPanel.IsVisible = false;
            ReservasPanel.IsVisible = false;
            FrameCaixasVeiculos.IsVisible = false;
            AlugarVeiculoPanel.IsVisible = false;

            try
            {
                // Filtrar reservas disponíveis
                var reservasDisponiveis = Reservas?.Where(r => r.IsDisponivel).ToList();

                // Verificar se há reservas disponíveis
                if (reservasDisponiveis == null || !reservasDisponiveis.Any())
                {
                    DisplayAlert("Atenção", "Nenhuma reserva disponível para cancelar.", "OK");
                    return;
                }

                // Atualizar o grid de reservas disponíveis para cancelamento
                AtualizarGridReservas(reservasDisponiveis);

                // Exibir o painel de cancelamento
                FrameCancelarReservas.IsVisible = true;
                FrameReservas.IsVisible = true;
                FrameConfirmacao.IsVisible = false;
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro", $"Falha ao carregar reservas para cancelamento: {ex.Message}", "OK");
            }
        }


     private void AtualizarGridReservas(List<Reserva> reservasDisponiveis)
{
    GridReservas.Children.Clear();

    if (reservasDisponiveis == null || !reservasDisponiveis.Any())
    {
        FrameReservas.IsVisible = false;
        return;
    }

    FrameReservas.IsVisible = true;
    int coluna = 0, linha = 0;

    foreach (var reserva in reservasDisponiveis)
    {
        var stackLayout = new StackLayout
        {
            Children =
            {
                new Label
                {
                    Text = $"{reserva.VeiculoTipo}: {reserva.ModeloVeiculo}",
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold
                },
                new Label
                {
                    Text = $"Retirada: {reserva.DataRetirada:dd/MM/yyyy} às {reserva.HoraRetirada}",
                    TextColor = Colors.LightGray
                },
                new Label
                {
                    Text = $"Devolução: {reserva.DataDevolucao:dd/MM/yyyy} às {reserva.HoraDevolucao}",
                    TextColor = Colors.LightGray
                }
            }
        };

        var frame = new Frame
        {
            Content = stackLayout,
            BackgroundColor = Colors.DarkGray,
            BorderColor = Colors.Transparent,
            CornerRadius = 10,
            Padding = 5,
            Margin = new Thickness(5)
        };

        frame.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => SelecionarReserva(reserva, frame))
        });

        Grid.SetRow(frame, linha);
        Grid.SetColumn(frame, coluna);
        GridReservas.Children.Add(frame);

        coluna++;
        if (coluna >= 6)
        {
            coluna = 0;
            linha++;
        }
    }
}



        private void SelecionarReserva(Reserva reserva, Frame frame)
        {
            reservaSelecionada = reserva;

            foreach (var child in GridReservas.Children.OfType<Frame>())
            {
                child.BorderColor = Colors.Transparent;
            }
            frame.BorderColor = Colors.Red;

            FrameReservas.IsVisible = false;
            FrameConfirmacao.IsVisible = true;
        }

        private void OnCancelConfirmacaoClicked(object sender, EventArgs e)
        {
            FrameReservas.IsVisible = true;
            FrameConfirmacao.IsVisible = false;
            reservaSelecionada = null;
        }

        private async void OnConfirmarCancelamentoClicked(object sender, EventArgs e)
        {
            if (reservaSelecionada != null)
            {
                // Solicita o motivo da desativação
                string motivo = await DisplayPromptAsync(
                    "Motivo da Desativação",
                    "Informe o motivo para desativar a reserva:",
                    "Confirmar",
                    "Cancelar",
                    placeholder: "Digite o motivo aqui"
                );

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    await DisplayAlert("Erro", "É necessário informar um motivo para desativar a reserva.", "OK");
                    return;
                }

                try
                {
                    // Atualiza a reserva como indisponível e adiciona o motivo da exclusão
                    reservaSelecionada.IsDisponivel = false;
                    reservaSelecionada.MotivoExclusao = motivo;

                    await reservaData.AtualizarReservaAsync(reservaSelecionada);

                    // Notifica o cliente
                    await NotificarUsuarioReservaExcluida(reservaSelecionada.Id, motivo);

                    await DisplayAlert("Sucesso", "Reserva desativada com sucesso.", "OK");

                    // Atualiza a interface
                    CarregarReservas();
                    FrameReservas.IsVisible = true;
                    FrameConfirmacao.IsVisible = false;
                    reservaSelecionada = null;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Falha ao desativar a reserva: {ex.Message}", "OK");
                }
            }
        }

        private async Task NotificarUsuarioReservaExcluida(int reservaId, string motivo)
        {
            try
            {
                var reserva = await reservaData.ObterReservaPorIdAsync(reservaId);
                if (reserva != null)
                {
                    reserva.MotivoExclusao = motivo;
                    await reservaData.AtualizarReservaAsync(reserva);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao notificar o cliente: {ex.Message}", "OK");
            }
        }

        private void OnToggleReservasCanceladasClicked(object sender, EventArgs e)
        {
            exibirReservasCanceladas = !exibirReservasCanceladas;
            ToggleCanceladasButton.Text = exibirReservasCanceladas ? "Ocultar Reservas Canceladas" : "Mostrar Reservas Canceladas";
            CarregarReservasNovo(); // Substituído para utilizar a nova função
        }




    }
}
