using AlocacaoVeiuculo.Modelo;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AlocacaoVeiuculo
{
    public partial class MainPage : ContentPage
    {
        private VeiculoRepositorio veiculoRepositorio = new();

        private string localRetirada;
        private DateTime dataRetirada;
        private TimeSpan horaRetirada;
        private DateTime dataDevolucao;
        private TimeSpan horaDevolucao;
        private string residencia;

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
        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            try
            {
                var cadastrarUsuarioPage = new CadastrarUsuarioPage(new UsuarioRepositorio());
                await Navigation.PushAsync(cadastrarUsuarioPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
        private async void OnCadastrarVeiculoClicked(object sender, EventArgs e)
        {
            try
            {
                var cadastrarVeiculoPage = new CadastrarVeiculoPage(veiculoRepositorio);
                await Navigation.PushAsync(cadastrarVeiculoPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
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

            var veiculosEncontrados = await veiculoRepositorio.PesquisarVeiculos(localRetirada, dataRetirada, dataDevolucao, residencia);

            if (veiculosEncontrados.Count > 0)
            {
                string resultado = string.Join("\n", veiculosEncontrados.Select(v =>
                {
                    if (v is Carro carro)
                        return $"Carro: {carro.Modelo} - Placa: {carro.Placa}";
                    if (v is Moto moto)
                        return $"Moto: {moto.Modelo} - Placa: {moto.Placa}";
                    return string.Empty;
                }));

                await DisplayAlert("Veículos Encontrados", resultado, "OK");
            }
            else
            {
                await DisplayAlert("Nenhum veículo encontrado", "Nenhum veículo atende aos critérios de pesquisa.", "OK");
            }
        }
    }
}
