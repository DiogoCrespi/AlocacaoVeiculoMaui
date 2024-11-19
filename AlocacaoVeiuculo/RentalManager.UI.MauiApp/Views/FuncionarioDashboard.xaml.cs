


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

            public FuncionarioDashboard()
            {
                InitializeComponent();
                reservaData = new ReservaData();
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
                try
                {
                    var todasReservas = await reservaData.ObterReservasAsync();

                    if (todasReservas == null || !todasReservas.Any())
                    {
                        await DisplayAlert("Aten��o", "Nenhuma reserva encontrada.", "OK");
                        FrameGerenciarReservas.IsVisible = false;
                        return;
                    }

                    // Limpa as reservas previamente carregadas
                    StackLayoutReservas.Children.Clear();

                    foreach (var reserva in todasReservas)
                    {
                        string modelo = "Modelo n�o encontrado";

                        // Busca o modelo do ve�culo associado � reserva
                        if (reserva.VeiculoTipo == "Carro")
                        {
                            var carro = await new CarroData().ObterCarroPorIdAsync(reserva.VeiculoId);
                            modelo = carro?.Modelo ?? modelo;
                        }
                        else if (reserva.VeiculoTipo == "Moto")
                        {
                            var moto = await new MotoData().ObterMotoPorIdAsync(reserva.VeiculoId);
                            modelo = moto?.Modelo ?? modelo;
                        }

                        // Cria o layout para cada reserva
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
                                Text = $"Cliente ID: {reserva.UsuarioId}",
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
                            },
                            new Button
                            {
                                Text = "Cancelar Reserva",
                                BackgroundColor = Colors.Red,
                                TextColor = Colors.White,
                                Command = new Command(async () => await CancelarReserva(reserva.Id))
                            }
                        }
                        };

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
                    await reservaData.RemoverReservaAsync(reservaId);
                    await DisplayAlert("Sucesso", "Reserva cancelada com sucesso.", "OK");

                    // Atualiza a lista de reservas ap�s o cancelamento
                    OnGerenciarReservasClicked(null, null);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Falha ao cancelar a reserva: {ex.Message}", "OK");
                }
            }
        }
    }

