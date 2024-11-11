using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Data;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AlocacaoVeiuculo
{
    public partial class CadastrarVeiculoPage : ContentPage
    {
        private CarroData carroData;
        private MotoData motoData;
        private ModeloVeiculoData modeloVeiculoData;

        public CadastrarVeiculoPage()
        {
            InitializeComponent();
            carroData = new CarroData();
            motoData = new MotoData();
            modeloVeiculoData = new ModeloVeiculoData();

            CarregarModelosAsync();

            var anos = Enumerable.Range(1980, DateTime.Now.Year - 1979).ToList();
            pickerAnoCarro.ItemsSource = anos;
            pickerAnoMoto.ItemsSource = anos;
        }

        private async Task CarregarModelosAsync()
        {
            var modelos = await modeloVeiculoData.ObterModelosAsync();
            var modelosCarro = modelos.Where(m => m.Tipo == "Carro").Select(m => m.Nome).ToList();
            var modelosMoto = modelos.Where(m => m.Tipo == "Moto").Select(m => m.Nome).ToList();

            pickerModeloCarro.ItemsSource = modelosCarro;
            pickerModeloMoto.ItemsSource = modelosMoto;
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

        private async void OnSalvarClicked(object sender, EventArgs e)
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

                await carroData.AdicionarCarroAsync(carro);
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

                await motoData.AdicionarMotoAsync(moto);
                await DisplayAlert("Cadastro Realizado", $"Veículo cadastrado: {moto.Modelo} - {moto.Placa}", "OK");
            }

            var carrosCadastrados = string.Join("\n", (await carroData.ObterCarrosAsync()).Select(c => $"{c.Modelo} - {c.Placa}"));
            var motosCadastradas = string.Join("\n", (await motoData.ObterMotosAsync()).Select(m => $"{m.Modelo} - {m.Placa}"));

            await DisplayAlert("Veículos Cadastrados", $"Carros:\n{carrosCadastrados}\n\nMotos:\n{motosCadastradas}", "OK");
            await Navigation.PopAsync();
        }
    }
}
