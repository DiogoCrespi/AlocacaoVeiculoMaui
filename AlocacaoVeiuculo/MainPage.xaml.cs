using AlocacaoVeiuculo.Data;
using AlocacaoVeiuculo.Modelo;
using Microsoft.Maui.Controls;
using System;

namespace AlocacaoVeiuculo
{
    public partial class MainPage : ContentPage
    {
        private string localRetirada;
        private DateTime dataRetirada;
        private TimeSpan horaRetirada;
        private DateTime dataDevolucao;
        private TimeSpan horaDevolucao;
        private string residencia;
        private Usuario usuarioLogado;

        public MainPage()
        {
            InitializeComponent();

            localRetirada = string.Empty;
            dataRetirada = DateTime.Now;
            horaRetirada = DateTime.Now.TimeOfDay;
            dataDevolucao = DateTime.Now;
            horaDevolucao = DateTime.Now.TimeOfDay;
            residencia = "Brasil";
        }

        private async void OnPesquisarClicked(object sender, EventArgs e)
        {
            localRetirada = entryLocalRetirada.Text;
            dataRetirada = datePickerRetirada.Date;
            horaRetirada = timePickerRetirada.Time;
            dataDevolucao = datePickerDevolucao.Date;
            horaDevolucao = timePickerDevolucao.Time;

            if (pickerResidencia.SelectedItem != null)
            {
                residencia = pickerResidencia.SelectedItem.ToString();
            }
            else
            {
                residencia = "Brasil";
            }

            var reserva = new Reserva
            {
                LocalRetirada = localRetirada,
                DataRetirada = dataRetirada,
                HoraRetirada = horaRetirada,
                DataDevolucao = dataDevolucao,
                HoraDevolucao = horaDevolucao,
                Residencia = residencia
            };

            var reservaData = new ReservaData();
            await reservaData.AdicionarReservaAsync(reserva);

            var disponibilidadeData = new DisponibilidadeData();
            var veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(dataRetirada, horaRetirada, dataDevolucao, horaDevolucao);

            string resultado = $"Local de Retirada: {localRetirada}\n" +
                               $"Data de Retirada: {dataRetirada.ToShortDateString()} às {horaRetirada}\n" +
                               $"Data de Devolução: {dataDevolucao.ToShortDateString()} às {horaDevolucao}\n" +
                               $"Residência: {residencia}\n\n" +
                               "Veículos Disponíveis:\n";

            if (veiculosDisponiveis.Any())
            {
                foreach (var disponibilidade in veiculosDisponiveis)
                {
                    if (disponibilidade.TipoVeiculo == "Carro")
                    {
                        var carroData = new CarroData();
                        var carro = await carroData.ObterCarroPorIdAsync(disponibilidade.VeiculoId);
                        if (carro != null)
                        {
                            resultado += $"Carro: {carro.Modelo}\n" +
                                         $"Placa: {carro.Placa}\n" +
                                         $"Ano: {carro.Ano}\n" +
                                         $"Quilometragem: {carro.Quilometragem}\n" +
                                         $"Combustível: {carro.TipoCombustivel}\n" +
                                         $"Portas: {carro.NumeroPortas}\n\n";
                        }
                    }
                    else if (disponibilidade.TipoVeiculo == "Moto")
                    {
                        var motoData = new MotoData();
                        var moto = await motoData.ObterMotoPorIdAsync(disponibilidade.VeiculoId);
                        if (moto != null)
                        {
                            resultado += $"Moto: {moto.Modelo}\n" +
                                         $"Placa: {moto.Placa}\n" +
                                         $"Ano: {moto.Ano}\n" +
                                         $"Quilometragem: {moto.Quilometragem}\n" +
                                         $"Combustível: {moto.TipoCombustivel}\n\n";
                        }
                    }
                }
            }
            else
            {
                resultado += "Nenhum veículo disponível para o período selecionado.";
            }

            await DisplayAlert("Opções Selecionadas", resultado, "OK");
        }
    

    public void MostrarBotaoUsuarioLogado(Usuario usuario)
        {
            usuarioLogado = usuario;
            btnUsuarioLogado.IsVisible = true;
        }

        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            try
            {
                var cadastrarUsuarioPage = new CadastrarUsuarioPage(this);
                await Navigation.PushAsync(cadastrarUsuarioPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void OnUsuarioLogadoClicked(object sender, EventArgs e)
        {
            if (usuarioLogado != null)
            {
                string mensagem = $"Nome: {usuarioLogado.Nome}\nCPF: {usuarioLogado.Cpf}\nData de Nascimento: {usuarioLogado.DataNascimento.ToShortDateString()}\nTelefone: {usuarioLogado.Telefone}";

                var acao = await DisplayActionSheet(mensagem, "Cancelar", null, "Minhas Reservas", "Sair");

                if (acao == "Minhas Reservas")
                {
                    // Código para abrir a tela de "Minhas Reservas"
                    await DisplayAlert("Ação", "Minhas Reservas selecionado.", "OK");
                }
                else if (acao == "Sair")
                {
                    usuarioLogado = null;
                    btnUsuarioLogado.IsVisible = false;
                    await DisplayAlert("Logout", "Você saiu da conta.", "OK");
                }
            }
        }

        private async void OnCadastrarVeiculoClicked(object sender, EventArgs e)
        {
            try
            {
                var cadastrarVeiculoPage = new CadastrarVeiculoPage();
                await Navigation.PushAsync(cadastrarVeiculoPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}
