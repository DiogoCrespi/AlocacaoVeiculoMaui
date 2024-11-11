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

        public void MostrarBotaoUsuarioLogado(Usuario usuario)
        {
            usuarioLogado = usuario;
            btnUsuarioLogado.IsVisible = true;
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

            string resultado = $"Local de Retirada: {localRetirada}\n" +
                               $"Data de Retirada: {dataRetirada.ToShortDateString()} às {horaRetirada}\n" +
                               $"Data de Devolução: {dataDevolucao.ToShortDateString()} às {horaDevolucao}\n" +
                               $"Residência: {residencia}";

            await DisplayAlert("Opções Selecionadas", resultado, "OK");
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
