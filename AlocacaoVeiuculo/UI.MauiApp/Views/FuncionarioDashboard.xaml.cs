using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using AlocacaoVeiuculo.Data.Reservations;
using AlocacaoVeiuculo.Data.Vehicles;

namespace AlocacaoVeiuculo.Pages
    {
        public partial class FuncionarioDashboard : ContentPage
        {
            private ReservaData reservaData;
            private Reserva reservaSelecionada;
            private bool exibirCanceladas = true;

        public FuncionarioDashboard()
            {
                InitializeComponent();
                reservaData = new ReservaData();
            }



        private async void OnGerarRelatoriosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Relatórios", "Opção de relatórios ainda será implementada.", "OK");

        }

        private async void OnGerenciarVeiculosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Veículos", "Opção de gerenciamento de veículos ainda será implementada.", "OK");
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

