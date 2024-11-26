using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.Data.Reservations;
using AlocacaoVeiuculo.Data.Vehicles;
using Microsoft.Maui;
using AlocacaoVeiuculo.Data.User;
using Microsoft.Maui.Controls;
using AlocacaoVeiuculo.Data.Vehicles;
using AlocacaoVeiuculo.Data.Reservations;
using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using System.Linq;
using System.Threading.Tasks;
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
            await DisplayAlert("Relatórios", "Gerando relatório...", "OK");

            // Obter dados de cada tabela do banco
            var carros = await carroData.ObterCarrosAsync();
            var motos = await motoData.ObterMotosAsync();
            var disponibilidades = await disponibilidadeData.ObterDisponibilidadesAsync();
            var reservas = await reservaData.ObterTodasReservasAsync();
            var usuarios = await new UsuarioData().ObterUsuariosAsync();

            // Limpar o conteúdo do Frame de Relatório
            var stackLayout = new StackLayout { Spacing = 15 };

            // Adicionar título "Usuários"
            stackLayout.Children.Add(new Label
            {
                Text = "Usuários:",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.White
            });
            foreach (var usuario in usuarios)
            {
                stackLayout.Children.Add(new Label
                {
                    Text = $"- Nome: {usuario.Nome}, CPF: {usuario.Cpf}, Telefone: {usuario.Telefone}, Disponível: {usuario.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar título "Carros"
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
                    Text = $"- Modelo: {carro.Modelo}, Placa: {carro.Placa}, Ano: {carro.Ano}, Quilometragem: {carro.Quilometragem}, Combustível: {carro.TipoCombustivel}, Disponível: {carro.IsDisponivel}, Alugado: {carro.IsAlugado}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar título "Motos"
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
                    Text = $"- Modelo: {moto.Modelo}, Placa: {moto.Placa}, Ano: {moto.Ano}, Quilometragem: {moto.Quilometragem}, Combustível: {moto.TipoCombustivel}, Disponível: {moto.IsDisponivel}, Alugado: {moto.IsAlugado}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar título "Disponibilidades"
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
                    Text = $"- Veículo: {disponibilidade.Modelo}, Tipo: {disponibilidade.TipoVeiculo}, Início: {disponibilidade.DataInicio:dd/MM/yyyy} às {disponibilidade.HoraInicio}, Fim: {disponibilidade.DataFim:dd/MM/yyyy} às {disponibilidade.HoraFim}, Disponível: {disponibilidade.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Adicionar título "Reservas"
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
                    Text = $"- Cliente ID: {reserva.UsuarioId}, Veículo: {reserva.ModeloVeiculo} ({reserva.VeiculoTipo}), Retirada: {reserva.DataRetirada:dd/MM/yyyy} às {reserva.HoraRetirada}, Devolução: {reserva.DataDevolucao:dd/MM/yyyy} às {reserva.HoraDevolucao}, Motivo Exclusão: {(string.IsNullOrEmpty(reserva.MotivoExclusao) ? "N/A" : reserva.MotivoExclusao)}, Disponível: {reserva.IsDisponivel}",
                    FontSize = 14,
                    TextColor = Colors.LightGray
                });
            }

            // Atualizar o FrameRelatorio com o conteúdo gerado
            FrameRelatorio.Content = new ScrollView { Content = stackLayout };
            FrameRelatorio.IsVisible = true;
        }




        private async void OnCadasVeiculosClicked(object sender, EventArgs e)
        {
            try
            {
                var gerenciarVeiculosPage = new CadastrarVeiculoPage(); // Instancie a página desejada
                await Navigation.PushAsync(gerenciarVeiculosPage); // Navega para a página de gerenciamento de veículos
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void OnGerenciarVeiculosClicked(object sender, EventArgs e)
        {
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
                    await DisplayAlert("Atenção", "Nenhuma reserva encontrada.", "OK");
                    FrameGerenciarReservas.IsVisible = false;
                    return;
                }

                StackLayoutReservas.Children.Clear();

                foreach (var reserva in todasReservas.Where(r => r.IsDisponivel || exibirCanceladas))
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
                        Text = $"Retirada: {reserva.DataRetirada:dd/MM/yyyy} às {reserva.HoraRetirada}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    },
                    new Label
                    {
                        Text = $"Devolução: {reserva.DataDevolucao:dd/MM/yyyy} às {reserva.HoraDevolucao}",
                        TextColor = Colors.LightGray,
                        FontSize = 14
                    }
                }
                    };

                    if (!reserva.IsDisponivel)
                    {
                        stackLayout.Children.Add(new Label
                        {
                            Text = $"Motivo da Exclusão: {reserva.MotivoExclusao}",
                            TextColor = Colors.Red,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14
                        });
                    }
                    else if (!string.IsNullOrWhiteSpace(reserva.MotivoModificacao))
                    {
                        stackLayout.Children.Add(new Label
                        {
                            Text = $"Algumas informações foram mudadas pois: {reserva.MotivoModificacao}",
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
                    // Envia notificação para o cliente, exibe destaque no painel ou registra no banco de dados
                    reserva.MotivoExclusao = motivo;
                    await reservaData.AtualizarReservaAsync(reserva);

                    await DisplayAlert(
                        "Notificação ao Cliente",
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

                // Exibe o frame de modificação com os campos preenchidos
                FrameModificarReserva.IsVisible = true;

                // Preenche os campos com as informações atuais da reserva
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
                await DisplayAlert("Erro", $"Falha ao preparar a modificação: {ex.Message}", "OK");
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
                        await DisplayAlert("Erro", "Por favor, informe o motivo da modificação.", "OK");
                        return;
                    }

                    // Atualiza os dados da reserva
                    reservaSelecionada.LocalRetirada = EntryNovoLocalRetirada.Text;
                    reservaSelecionada.DataRetirada = DatePickerNovaDataRetirada.Date;
                    reservaSelecionada.HoraRetirada = TimePickerNovaHoraRetirada.Time;
                    reservaSelecionada.DataDevolucao = DatePickerNovaDataDevolucao.Date;
                    reservaSelecionada.HoraDevolucao = TimePickerNovaHoraDevolucao.Time;

                    // Adiciona o motivo da modificação
                    reservaSelecionada.MotivoModificacao = EntryMotivoModificacao.Text;

                    // Atualiza no banco de dados
                    await reservaData.AtualizarReservaAsync(reservaSelecionada);

                    await DisplayAlert("Sucesso", "Reserva modificada com sucesso. O cliente será informado.", "OK");

                    // Oculta o frame de modificação e exibe o StackLayout de reservas
                    FrameModificarReserva.IsVisible = false;
                    StackLayoutReservas.IsVisible = true;

                    // Atualiza a lista de reservas
                    OnGerenciarReservasClicked(null, null);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar as modificações: {ex.Message}", "OK");
            }
        }





    }
}

