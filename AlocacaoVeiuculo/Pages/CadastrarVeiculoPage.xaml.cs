using AlocacaoVeiuculo.Modelo;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AlocacaoVeiuculo
{
    public partial class CadastrarVeiculoPage : ContentPage
    {
        private VeiculoRepositorio veiculoRepositorio;
        private ModeloRepositorio modeloRepositorio;

        public CadastrarVeiculoPage(VeiculoRepositorio repositorio)
        {
            InitializeComponent();
            veiculoRepositorio = repositorio;
            modeloRepositorio = new ModeloRepositorio();

            // Preencher os pickers de modelo de carro e moto
            pickerModeloCarro.ItemsSource = modeloRepositorio.ObterModelosCarro();
            pickerModeloMoto.ItemsSource = modeloRepositorio.ObterModelosMoto();

            // Populando os pickers de ano com um intervalo de anos (ex: de 1980 a 2024)
            var anos = Enumerable.Range(1980, DateTime.Now.Year - 1979).ToList();
            pickerAnoCarro.ItemsSource = anos;
            pickerAnoMoto.ItemsSource = anos;
        }

        private void OnTipoVeiculoChanged(object sender, EventArgs e)
        {
            if (pickerTipoVeiculo.SelectedItem.ToString() == "Carro")
            {
                carroSection.IsVisible = true;
                motoSection.IsVisible = false;
            }
            else if (pickerTipoVeiculo.SelectedItem.ToString() == "Moto")
            {
                carroSection.IsVisible = false;
                motoSection.IsVisible = true;
            }
        }

        private async void OnSalvarClicked(object sender, EventArgs e, Moto moto)
        {
            if (pickerTipoVeiculo.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Selecione o tipo de veículo.", "OK");
                return;
            }

            if (pickerTipoVeiculo.SelectedItem.ToString() == "Carro")
            {
                if (pickerModeloCarro.SelectedItem == null ||
                    pickerAnoCarro.SelectedItem == null ||
                    string.IsNullOrEmpty(entryQuilometragemCarro.Text) ||
                    pickerTipoCombustivelCarro.SelectedItem == null ||
                    pickerNumeroPortasCarro.SelectedItem == null)
                {
                    await DisplayAlert("Erro", "Preencha todos os campos corretamente.", "OK");
                    return;
                }

                var carro = new Carro
                {
                    Placa = entryPlacaCarro.Text,
                    Modelo = pickerModeloCarro.SelectedItem.ToString(),
                    Ano = (int)pickerAnoCarro.SelectedItem,
                    Quilometragem = double.Parse(entryQuilometragemCarro.Text),
                    TipoCombustivel = pickerTipoCombustivelCarro.SelectedItem.ToString(),
                    NumeroPortas = int.Parse(pickerNumeroPortasCarro.SelectedItem.ToString())
                };

                veiculoRepositorio.AdicionarCarro(carro);
                await DisplayAlert("Cadastro Realizado", $"Veículo cadastrado: {carro.Modelo} - {carro.Placa}", "OK");
            }
            else if (pickerTipoVeiculo.SelectedItem.ToString() == "Moto")
            {
                if (pickerModeloMoto.SelectedItem == null ||
                    pickerAnoMoto.SelectedItem == null ||
                    string.IsNullOrEmpty(entryQuilometragemMoto.Text) ||
                    pickerTipoCombustivelMoto.SelectedItem == null)
                {
                    await DisplayAlert("Erro", "Preencha todos os campos corretamente.", "OK");
                    return;
                }

                var moto = new Moto
                {
                    Placa = entryPlacaMoto.Text,
                    Modelo = pickerModeloMoto.SelectedItem.ToString(),
                    Ano = (int)pickerAnoMoto.SelectedItem,
                    Quilometragem = double.Parse(entryQuilometragemMoto.Text),
                    TipoCombustivel = pickerTipoCombustivelMoto.SelectedItem.ToString()
                };

                veiculoRepositorio.AdicionarMoto(moto);
                await DisplayAlert("Cadastro Realizado", $"Veículo cadastrado: {moto.Modelo} - {moto.Placa}", "OK");
            }

            // Mostrar todos os veículos cadastrados
            string carrosCadastrados = string.Join("\n", veiculoRepositorio.ObterCarros().ConvertAll(c => $"{c.Modelo} - {c.Placa}"));
            string motosCadastradas = string.Join("\n", veiculoRepositorio.ObterMotos().ConvertAll(m => $"{m.Modelo} - {m.Placa}"));

            await DisplayAlert("Veículos Cadastrados", $"Carros:\n{carrosCadastrados}\n\nMotos:\n{motosCadastradas}", "OK");
            await Navigation.PopAsync();
        }
    }
}
