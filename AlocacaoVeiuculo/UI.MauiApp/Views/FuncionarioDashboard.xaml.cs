
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;

using AlocacaoVeiuculo.Data.Reservations;
using AlocacaoVeiuculo.Data.Vehicles;

using AlocacaoVeiuculo.Data.User;

using SQLite;


namespace AlocacaoVeiuculo.Pages
    {
        public partial class FuncionarioDashboard : ContentPage
        {
        private DisponibilidadeData disponibilidadeData = new DisponibilidadeData();
        private ReservaData reservaData;
        private Reserva reservaSelecionada;
        private readonly CarroData carroData;
        private readonly MotoData motoData;
        //private readonly DisponibilidadeData disponibilidadeData;
       // private readonly ReservaData reservaData;
        private readonly SQLiteAsyncConnection database;

        private bool exibirCanceladas = true;

        public FuncionarioDashboard()
            {
                InitializeComponent();
                carroData = new CarroData();
                motoData = new MotoData();
                disponibilidadeData = new DisponibilidadeData();
                reservaData = new ReservaData();
        }


        private async void OnGerarRelatoriosClicked(object sender, EventArgs e)
        {
           // await DisplayAlert("Relat�rios", "Gerando relat�rio...", "OK");

            // Obter dados de cada tabela do banco
            var carros = await carroData.ObterCarrosAsync();
            var motos = await motoData.ObterMotosAsync();
            var disponibilidades = await disponibilidadeData.ObterDisponibilidadesAsync();
            var reservas = await reservaData.ObterTodasReservasAsync();
            var usuarios = await new UsuarioData().ObterUsuariosAsync();

            // Limpar o conte�do do Frame de Relat�rio
            var stackLayout = new StackLayout { Spacing = 15 };

            // Adicionar t�tulo "Usu�rios"
            stackLayout.Children.Add(new Label
            {
                Text = "Usu�rios:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var usuario in usuarios)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Nome: {usuario.Nome}, CPF: {usuario.Cpf}, Telefone: {usuario.Telefone}, Dispon�vel: {usuario.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar t�tulo "Carros"
            stackLayout.Children.Add(new Label
            {
                Text = "Carros:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var carro in carros)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Modelo: {carro.Modelo}, Placa: {carro.Placa}, Ano: {carro.Ano}, Quilometragem: {carro.Quilometragem}, Combust�vel: {carro.TipoCombustivel}, Dispon�vel: {carro.IsDisponivel}, Alugado: {carro.IsAlugado}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar t�tulo "Motos"
            stackLayout.Children.Add(new Label
            {
                Text = "Motos:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var moto in motos)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Modelo: {moto.Modelo}, Placa: {moto.Placa}, Ano: {moto.Ano}, Quilometragem: {moto.Quilometragem}, Combust�vel: {moto.TipoCombustivel}, Dispon�vel: {moto.IsDisponivel}, Alugado: {moto.IsAlugado}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar t�tulo "Disponibilidades"
            stackLayout.Children.Add(new Label
            {
                Text = "Disponibilidades:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var disponibilidade in disponibilidades)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Ve�culo: {disponibilidade.Modelo}, Tipo: {disponibilidade.TipoVeiculo}, In�cio: {disponibilidade.DataInicio:dd/MM/yyyy} �s {disponibilidade.HoraInicio}, Fim: {disponibilidade.DataFim:dd/MM/yyyy} �s {disponibilidade.HoraFim}, Dispon�vel: {disponibilidade.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar t�tulo "Reservas"
            stackLayout.Children.Add(new Label
            {
                Text = "Reservas:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var reserva in reservas)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Cliente ID: {reserva.UsuarioId}, Ve�culo: {reserva.ModeloVeiculo} ({reserva.VeiculoTipo}), Retirada: {reserva.DataRetirada:dd/MM/yyyy} �s {reserva.HoraRetirada}, Devolu��o: {reserva.DataDevolucao:dd/MM/yyyy} �s {reserva.HoraDevolucao}, Motivo Exclus�o: {(string.IsNullOrEmpty(reserva.MotivoExclusao) ? "N/A" : reserva.MotivoExclusao)}, Dispon�vel: {reserva.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Atualizar o FrameRelatorio com o conte�do gerado
            FrameRelatorio.Content = new ScrollView { Content = stackLayout };
            FrameRelatorio.IsVisible = true;
        }




        private async void OnCadasVeiculosClicked(object sender, EventArgs e)
        {
            try
            {
                var gerenciarVeiculosPage = new CadastrarVeiculoPage(); // Instancie a p�gina desejada
                await Navigation.PushAsync(gerenciarVeiculosPage); // Navega para a p�gina de gerenciamento de ve�culos
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void OnGerenciarVeiculosClicked(object sender, EventArgs e)
        {
            HeaderGrid.IsVisible = true;

            try
            {
                var carros = await carroData.ObterCarrosAsync();
                var motos = await motoData.ObterMotosAsync();

                if ((carros == null || !carros.Any()) && (motos == null || !motos.Any()))
                {
                    await DisplayAlert("Aten��o", "Nenhum ve�culo encontrado.", "OK");
                    FrameGerenciarVeiculos.IsVisible = false;
                    return;
                }

                StackLayoutVeiculos.Children.Clear();

                // Listando Carros
                if (carros != null && carros.Any())
                {
                    StackLayoutVeiculos.Children.Add(new Label
                    {
                        Text = "Carros",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Start
                    });

                    foreach (var carro in carros)
                    {
                        var stackLayout = new StackLayout
                        {
                            Margin = new Thickness(0, 10),
                            Children =
                    {
                        new Label
                        {
                            Text = $"Modelo: {carro.Modelo}",
                            TextColor = Colors.White,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16
                        },
                        new Label
                        {
                            Text = $"Placa: {carro.Placa}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Ano: {carro.Ano}, Quilometragem: {carro.Quilometragem}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Combust�vel: {carro.TipoCombustivel}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Alugado: {carro.IsAlugado}",
                            TextColor = carro.IsAlugado ? Colors.Red : Colors.Green,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Dispon�vel: {carro.IsDisponivel}",
                            TextColor = carro.IsDisponivel ? Colors.Green : Colors.Red,
                            FontSize = 14
                        },
                        // Informa��es de desativa��o ou modifica��o
                        carro.IsDisponivel ? null : new Label
                        {
                            Text = $"Motivo da Desativa��o: {carro.MotivoModificacao}",
                            TextColor = Colors.Red,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        },
                        !string.IsNullOrWhiteSpace(carro.MotivoModificacao) ? new Label
                        {
                            Text = $"Algumas informa��es foram modificadas pois: {carro.MotivoModificacao}",
                            TextColor = Colors.Yellow,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        } : null,
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 10,
                            Children =
                            {
                                new Button
                                {
                                    Text = "Cancelar",
                                    BackgroundColor = Colors.Red,
                                    TextColor = Colors.White,
                                    Command = new Command(async () => await CancelarVeiculo(carro.Id, "Carro"))
                                },
                                new Button
                                {
                                    Text = "Modificar",
                                    BackgroundColor = Colors.Blue,
                                    TextColor = Colors.White,
                                    Command = new Command(() => ModificarVeiculo(carro.Id, "Carro"))
                                }
                            }
                        }
                    }//.Where(child => child != null) // Remove elementos nulos
                        };

                        StackLayoutVeiculos.Children.Add(stackLayout);
                    }
                }

                // Listando Motos
                if (motos != null && motos.Any())
                {
                    StackLayoutVeiculos.Children.Add(new Label
                    {
                        Text = "Motos",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Start
                    });

                    foreach (var moto in motos)
                    {
                        var stackLayout = new StackLayout
                        {
                            Margin = new Thickness(0, 10),
                            Children =
                    {
                        new Label
                        {
                            Text = $"Modelo: {moto.Modelo}",
                            TextColor = Colors.White,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16
                        },
                        new Label
                        {
                            Text = $"Placa: {moto.Placa}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Ano: {moto.Ano}, Quilometragem: {moto.Quilometragem}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Combust�vel: {moto.TipoCombustivel}",
                            TextColor = Colors.LightGray,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Alugado: {moto.IsAlugado}",
                            TextColor = moto.IsAlugado ? Colors.Red : Colors.Green,
                            FontSize = 14
                        },
                        new Label
                        {
                            Text = $"Dispon�vel: {moto.IsDisponivel}",
                            TextColor = moto.IsDisponivel ? Colors.Green : Colors.Red,
                            FontSize = 14
                        },
                        // Informa��es de desativa��o ou modifica��o
                        moto.IsDisponivel ? null : new Label
                        {
                            Text = $"Motivo da Desativa��o: {moto.MotivoModificacao}",
                            TextColor = Colors.Red,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        },
                        !string.IsNullOrWhiteSpace(moto.MotivoModificacao) ? new Label
                        {
                            Text = $"Algumas informa��es foram modificadas pois: {moto.MotivoModificacao}",
                            TextColor = Colors.Yellow,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        } : null,
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 10,
                            Children =
                            {
                                new Button
                                {
                                    Text = "Cancelar",
                                    BackgroundColor = Colors.Red,
                                    TextColor = Colors.White,
                                    Command = new Command(async () => await CancelarVeiculo(moto.Id, "Moto"))
                                },
                                new Button
                                {
                                    Text = "Modificar",
                                    BackgroundColor = Colors.Blue,
                                    TextColor = Colors.White,
                                    Command = new Command(() => ModificarVeiculo(moto.Id, "Moto"))
                                }
                            }
                        }
                    }//.Where(child => child != null) // Remove elementos nulos
                        };

                        StackLayoutVeiculos.Children.Add(stackLayout);
                    }
                }

                FrameGerenciarVeiculos.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao carregar os ve�culos: {ex.Message}", "OK");
            }
        }



        private async Task CancelarVeiculo(int veiculoId, string tipoVeiculo)
        {
            try
            {
                string motivoCancelamento = await DisplayPromptAsync(
                    "Cancelar Ve�culo",
                    "Informe o motivo do cancelamento:",
                    "OK",
                    "Cancelar",
                    placeholder: "Digite o motivo aqui"
                );

                if (string.IsNullOrWhiteSpace(motivoCancelamento))
                {
                    await DisplayAlert("Erro", "O motivo do cancelamento � obrigat�rio.", "OK");
                    return;
                }

                if (tipoVeiculo == "Carro")
                {
                    var carro = await carroData.ObterCarroPorIdAsync(veiculoId);
                    if (carro != null)
                    {
                        carro.IsDisponivel = false;
                        await carroData.AtualizarCarroAsync(carro);
                    }
                }
                else if (tipoVeiculo == "Moto")
                {
                    var moto = await motoData.ObterMotoPorIdAsync(veiculoId);
                    if (moto != null)
                    {
                        moto.IsDisponivel = false;
                        await motoData.AtualizarMotoAsync(moto);
                    }
                }

                await DisplayAlert("Sucesso", $"{tipoVeiculo} cancelado com sucesso.", "OK");
                OnGerenciarVeiculosClicked(null, null);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao cancelar o ve�culo: {ex.Message}", "OK");
            }
        }

        private async void ModificarVeiculo(int veiculoId, string tipoVeiculo)
        {
            try
            {
                if (tipoVeiculo == "Carro")
                {
                    var carro = await carroData.ObterCarroPorIdAsync(veiculoId);
                    if (carro != null)
                    {
                        await ExibirFormularioModificacao(carro, tipoVeiculo);
                    }
                }
                else if (tipoVeiculo == "Moto")
                {
                    var moto = await motoData.ObterMotoPorIdAsync(veiculoId);
                    if (moto != null)
                    {
                        await ExibirFormularioModificacao(moto, tipoVeiculo);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar os dados do ve�culo: {ex.Message}", "OK");
            }
        }

        private async Task ExibirFormularioModificacao(object veiculo, string tipoVeiculo)
        {
            try
            {
                if (veiculo == null)
                {
                    await DisplayAlert("Erro", "Ve�culo inv�lido.", "OK");
                    return;
                }

                string veiculoModelo = tipoVeiculo == "Carro" ? ((Carro)veiculo).Modelo : ((Moto)veiculo).Modelo;
                string veiculoPlaca = tipoVeiculo == "Carro" ? ((Carro)veiculo).Placa : ((Moto)veiculo).Placa;

                string novoModelo = await DisplayPromptAsync(
                    "Modificar Ve�culo",
                    $"Modelo atual: {veiculoModelo}\nInforme o novo modelo:",
                    initialValue: veiculoModelo,
                    maxLength: 50);

                if (string.IsNullOrWhiteSpace(novoModelo))
                {
                    await DisplayAlert("Erro", "O modelo � obrigat�rio.", "OK");
                    return;
                }

                string novaPlaca = await DisplayPromptAsync(
                    "Modificar Ve�culo",
                    $"Placa atual: {veiculoPlaca}\nInforme a nova placa:",
                    initialValue: veiculoPlaca,
                    maxLength: 10);

                if (string.IsNullOrWhiteSpace(novaPlaca))
                {
                    await DisplayAlert("Erro", "A placa � obrigat�ria.", "OK");
                    return;
                }

                string motivoModificacao = await DisplayPromptAsync(
                    "Motivo da Modifica��o",
                    "Informe o motivo para a modifica��o:",
                    maxLength: 100);

                if (string.IsNullOrWhiteSpace(motivoModificacao))
                {
                    await DisplayAlert("Erro", "O motivo da modifica��o � obrigat�rio.", "OK");
                    return;
                }

                if (tipoVeiculo == "Carro")
                {
                    var carro = (Carro)veiculo;
                    carro.Modelo = novoModelo;
                    carro.Placa = novaPlaca;
                    carro.MotivoModificacao = motivoModificacao;

                    await carroData.AtualizarCarroAsync(carro);
                }
                else if (tipoVeiculo == "Moto")
                {
                    var moto = (Moto)veiculo;
                    moto.Modelo = novoModelo;
                    moto.Placa = novaPlaca;
                    moto.MotivoModificacao = motivoModificacao;

                    await motoData.AtualizarMotoAsync(moto);
                }

                await DisplayAlert("Sucesso", "Ve�culo modificado com sucesso.", "OK");
                OnGerenciarVeiculosClicked(null, null);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao modificar o ve�culo: {ex.Message}", "OK");
            }
        }




















        private void OnToggleReservasCanceladasClicked(object sender, EventArgs e)
        {
            exibirCanceladas = !exibirCanceladas;
            ToggleCanceladasButton.Text = exibirCanceladas ? "Ocultar Reservas Canceladas" : "Exibir Reservas Canceladas";
            OnGerenciarReservasClicked(null, null);
        }

        private async void OnGerenciarReservasClicked(object sender, EventArgs e)
        {

            HeaderGrid.IsVisible = true;

            try
            {
                var todasReservas = await reservaData.ObterReservasComClientesAsync();

                if (todasReservas == null || !todasReservas.Any())
                {
                    await DisplayAlert("Aten��o", "Nenhuma reserva encontrada.", "OK");
                    FrameGerenciarReservas.IsVisible = false;
                    return;
                }

                StackLayoutReservas.Children.Clear();

                foreach (var reserva in todasReservas.Where(r => r.IsDisponivel || exibirCanceladas))
                {
                    string modelo = reserva.VeiculoTipo == "Carro"
                        ? (await new CarroData().ObterCarroPorIdAsync(reserva.VeiculoId))?.Modelo ?? "Modelo n�o encontrado"
                        : (await new MotoData().ObterMotoPorIdAsync(reserva.VeiculoId))?.Modelo ?? "Modelo n�o encontrado";

                    var stackLayout = new StackLayout
                    {
                        Margin = new Thickness(0, 10),
                        Children =
                {
                    new Label
                    {
                        Text = $"{reserva.VeiculoTipo}: {modelo}",
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    },
                    new Label
                    {
                        Text = $"Cliente: {reserva.NomeCliente}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    },
                    new Label
                    {
                        Text = $"Local Retirada: {reserva.LocalRetirada}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    },
                    new Label
                    {
                        Text = $"Retirada: {reserva.DataRetirada:dd/MM/yyyy} �s {reserva.HoraRetirada}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    },
                    new Label
                    {
                        Text = $"Devolu��o: {reserva.DataDevolucao:dd/MM/yyyy} �s {reserva.HoraDevolucao}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    }
                }
                    };

                    if (!reserva.IsDisponivel)
                    {
                        stackLayout.Children.Add(new Label
                        {
                            Text = $"Motivo da Exclus�o: {reserva.MotivoExclusao}",
                            TextColor = Colors.Red,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        });
                    }
                    else if (!string.IsNullOrWhiteSpace(reserva.MotivoModificacao))
                    {
                        stackLayout.Children.Add(new Label
                        {
                            Text = $"Algumas informa��es foram mudadas pois: {reserva.MotivoModificacao}",
                            TextColor = Colors.Yellow,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        });
                    }

                    if (reserva.IsDisponivel)
                    {
                        stackLayout.Children.Add(new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 10,
                            Children =
                    {
                        new Button
                        {
                            Text = "Cancelar Reserva",
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            Command = new Command(async () => await CancelarReserva(reserva.Id))
                        },
                        new Button
                        {
                            Text = "Modificar Reserva",
                            BackgroundColor = Colors.Blue,
                            TextColor = Colors.White,
                            Command = new Command(async () => await ModificarReserva(reserva))
                        }
                    }
                        });
                    }

                    StackLayoutReservas.Children.Add(stackLayout);
                }

                FrameGerenciarReservas.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao carregar as reservas: {ex.Message}", "OK");
            }
        }

        private async Task CancelarReserva(int reservaId)
        {
            try
            {
                var reserva = await reservaData.ObterReservaPorIdAsync(reservaId);
                if (reserva == null)
                {
                    await DisplayAlert("Erro", "Reserva n�o encontrada.", "OK");
                    return;
                }

                string motivoExclusao = await DisplayPromptAsync(
                    "Motivo da Exclus�o",
                    "Por favor, informe o motivo para cancelar a reserva:",
                    "OK",
                    "Cancelar");

                if (string.IsNullOrWhiteSpace(motivoExclusao))
                {
                    await DisplayAlert("Erro", "O motivo da exclus�o � obrigat�rio.", "OK");
                    return;
                }

                reserva.IsDisponivel = false;
                reserva.MotivoExclusao = motivoExclusao;
                await reservaData.AtualizarReservaAsync(reserva);

                await DisplayAlert("Sucesso", "Reserva cancelada com sucesso.", "OK");

                await NotificarUsuarioReservaExcluida(reservaId, motivoExclusao);

                OnGerenciarReservasClicked(null, null);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao cancelar a reserva: {ex.Message}", "OK");
            }
        }

        private async Task NotificarUsuarioReservaExcluida(int reservaId, string motivo)
        {
            try
            {
                var reserva = await reservaData.ObterReservaPorIdAsync(reservaId);
                if (reserva != null)
                {
                    // Envia notifica��o para o cliente, exibe destaque no painel ou registra no banco de dados
                    reserva.MotivoExclusao = motivo;
                    await reservaData.AtualizarReservaAsync(reserva);

                    await DisplayAlert(
                        "Notifica��o ao Cliente",
                        $"A reserva foi cancelada. Motivo informado: {motivo}",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao notificar o cliente: {ex.Message}", "OK");
            }
        }

        private async Task ModificarReserva(Reserva reserva)
        {
            try
            {
             
                HeaderGrid.IsVisible = false;
                // Oculta a lista de reservas
                StackLayoutReservas.IsVisible = false;

                // Exibe o frame de modifica��o com os campos preenchidos
                FrameModificarReserva.IsVisible = true;

                // Preenche os campos com as informa��es atuais da reserva
                LabelLocalRetirada.Text = $"Local Atual: {reserva.LocalRetirada}";
                EntryNovoLocalRetirada.Text = reserva.LocalRetirada;

                LabelDataRetirada.Text = $"Data Atual: {reserva.DataRetirada:dd/MM/yyyy}";
                DatePickerNovaDataRetirada.Date = reserva.DataRetirada;

                LabelHoraRetirada.Text = $"Hora Atual: {reserva.HoraRetirada}";
                TimePickerNovaHoraRetirada.Time = reserva.HoraRetirada;

                LabelDataDevolucao.Text = $"Data Atual: {reserva.DataDevolucao:dd/MM/yyyy}";
                DatePickerNovaDataDevolucao.Date = reserva.DataDevolucao;

                LabelHoraDevolucao.Text = $"Hora Atual: {reserva.HoraDevolucao}";
                TimePickerNovaHoraDevolucao.Time = reserva.HoraDevolucao;

                // Guarda a reserva selecionada
                reservaSelecionada = reserva;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao preparar a modifica��o: {ex.Message}", "OK");
            }
        }

        private async void OnSalvarModificacoesClicked(object sender, EventArgs e)
        {
            try
            {
                if (reservaSelecionada != null)
                {
                    // Verifica se o motivo foi informado
                    if (string.IsNullOrWhiteSpace(EntryMotivoModificacao.Text))
                    {
                        await DisplayAlert("Erro", "Por favor, informe o motivo da modifica��o.", "OK");
                        return;
                    }

                    // Atualiza os dados da reserva
                    reservaSelecionada.LocalRetirada = EntryNovoLocalRetirada.Text;
                    reservaSelecionada.DataRetirada = DatePickerNovaDataRetirada.Date;
                    reservaSelecionada.HoraRetirada = TimePickerNovaHoraRetirada.Time;
                    reservaSelecionada.DataDevolucao = DatePickerNovaDataDevolucao.Date;
                    reservaSelecionada.HoraDevolucao = TimePickerNovaHoraDevolucao.Time;

                    // Adiciona o motivo da modifica��o
                    reservaSelecionada.MotivoModificacao = EntryMotivoModificacao.Text;

                    // Atualiza no banco de dados
                    await reservaData.AtualizarReservaAsync(reservaSelecionada);

                    await DisplayAlert("Sucesso", "Reserva modificada com sucesso. O cliente ser� informado.", "OK");

                    // Oculta o frame de modifica��o e exibe o StackLayout de reservas
                    FrameModificarReserva.IsVisible = false;
                    StackLayoutReservas.IsVisible = true;

                    // Atualiza a lista de reservas
                    OnGerenciarReservasClicked(null, null);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar as modifica��es: {ex.Message}", "OK");
            }
        }





    }
}

