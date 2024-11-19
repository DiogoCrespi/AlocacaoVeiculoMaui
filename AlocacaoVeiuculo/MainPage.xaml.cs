using AlocacaoVeiuculo.Data;
using AlocacaoVeiuculo.RentalManager.Model;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;


using AlocacaoVeiuculo.Data.Reservations;

using AlocacaoVeiuculo.Data.User;
using AlocacaoVeiuculo.Data.Vehicles;


using AlocacaoVeiuculo.Pages;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AlocacaoVeiuculo.RentalManager.Model.Users;

namespace AlocacaoVeiuculo
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private string localRetirada;
        private DateTime dataRetirada;
        private TimeSpan horaRetirada;
        private DateTime dataDevolucao;
        private TimeSpan horaDevolucao;
        private Usuario usuarioLogado;
        private bool isAdmin;

        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            localRetirada = string.Empty;
            dataRetirada = DateTime.Now;
            horaRetirada = DateTime.Now.TimeOfDay;
            dataDevolucao = DateTime.Now;
            horaDevolucao = DateTime.Now.TimeOfDay;

            IsAdmin = false;
        }

        private async void OnPesquisarClicked(object sender, EventArgs e)
        {
            localRetirada = entryLocalRetirada.Text;
            dataRetirada = datePickerRetirada.Date;
            horaRetirada = timePickerRetirada.Time;
            dataDevolucao = datePickerDevolucao.Date;
            horaDevolucao = timePickerDevolucao.Time;

            var reserva = new Reserva
            {
                LocalRetirada = localRetirada,
                DataRetirada = dataRetirada,
                HoraRetirada = horaRetirada,
                DataDevolucao = dataDevolucao,
                HoraDevolucao = horaDevolucao
            };

            var reservaData = new ReservaData();
            await reservaData.AdicionarReservaAsync(reserva);

            var disponibilidadeData = new DisponibilidadeData();
            var veiculosDisponiveis = await disponibilidadeData.ObterVeiculosDisponiveisAsync(dataRetirada, horaRetirada, dataDevolucao, horaDevolucao);

            string resultado = $"Local de Retirada: {localRetirada}\n" +
                               $"Data de Retirada: {dataRetirada.ToShortDateString()} às {horaRetirada}\n" +
                               $"Data de Devolução: {dataDevolucao.ToShortDateString()} às {horaDevolucao}\n\n" +
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

            bool continuar = await DisplayAlert("Opções Selecionadas", resultado, "Continuar com Aluguel?", "Cancelar");

            if (continuar)
            {
                if (usuarioLogado == null)
                {
                    await DisplayAlert("Login Necessário", "Por favor, faça login para continuar.", "OK");
                    await Navigation.PushAsync(new CadastrarUsuarioPage(this));
                }
                else
                {
                    await Navigation.PushAsync(new UsuarioReservas(usuarioLogado, localRetirada, dataRetirada, horaRetirada, dataDevolucao, horaDevolucao, veiculosDisponiveis));
                }
            }
        }


        public void MostrarBotaoUsuarioLogado(Usuario usuario)
        {
            usuarioLogado = usuario;
            btnUsuarioLogado.IsVisible = true;
            IsAdmin = usuario.Nome == "admin"; // Verifica se o usuário logado é o administrador
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
                    await Navigation.PushAsync(new UsuarioReservas(usuarioLogado));
                }
                else if (acao == "Sair")
                {
                    usuarioLogado = null;
                    btnUsuarioLogado.IsVisible = false;
                    IsAdmin = false; // Remove permissões de administrador
                    await DisplayAlert("Logout", "Você saiu da conta.", "OK");
                }
            }
        }





        //----------------cadastro
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //---------------Funcionario
        private async void OnAbrirPaginaFuncionarioClicked(object sender, EventArgs e)
        {
            try
            {
                var funcionarioDashboardPage = new FuncionarioDashboard();
                await Navigation.PushAsync(funcionarioDashboardPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }




    }
}
