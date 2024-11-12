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
        private DisponibilidadeData disponibilidadeData;

        public CadastrarVeiculoPage()
        {
            InitializeComponent();
            carroData = new CarroData();
            motoData = new MotoData();
            modeloVeiculoData = new ModeloVeiculoData();
            disponibilidadeData = new DisponibilidadeData();

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
        private async void OnTipoDisponibilidadeVeiculoChanged(object sender, EventArgs e)
        {
            disponibilidadeCarroSection.IsVisible = false;
            disponibilidadeMotoSection.IsVisible = false;

            if (pickerDisponibilidadeTipoVeiculo.SelectedItem?.ToString() == "Carro")
            {
                var carros = await carroData.ObterCarrosAsync();
                pickerDisponibilidadeCarro.ItemsSource = carros.Any() ? carros : new List<Carro> { new Carro { Modelo = "Nenhum veículo cadastrado" } };
                pickerDisponibilidadeCarro.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeCarroSection.IsVisible = true;
            }
            else if (pickerDisponibilidadeTipoVeiculo.SelectedItem?.ToString() == "Moto")
            {
                var motos = await motoData.ObterMotosAsync();
                pickerDisponibilidadeMoto.ItemsSource = motos.Any() ? motos : new List<Moto> { new Moto { Modelo = "Nenhum veículo cadastrado" } };
                pickerDisponibilidadeMoto.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeMotoSection.IsVisible = true;
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
        }

        private async void OnSalvarDisponibilidadeClicked(object sender, EventArgs e)
        {
            if (pickerDisponibilidadeTipoVeiculo.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Selecione o tipo de veículo.", "OK");
                return;
            }

            string tipoVeiculo = pickerDisponibilidadeTipoVeiculo.SelectedItem.ToString();
            int veiculoId;

            if (tipoVeiculo == "Carro")
            {
                var carroSelecionado = pickerDisponibilidadeCarro.SelectedItem as Carro;
                if (carroSelecionado == null || carroSelecionado.Modelo == "Nenhum veículo cadastrado")
                {
                    await DisplayAlert("Erro", "Selecione um veículo válido.", "OK");
                    return;
                }
                veiculoId = carroSelecionado.Id;
            }
            else if (tipoVeiculo == "Moto")
            {
                var motoSelecionado = pickerDisponibilidadeMoto.SelectedItem as Moto;
                if (motoSelecionado == null || motoSelecionado.Modelo == "Nenhum veículo cadastrado")
                {
                    await DisplayAlert("Erro", "Selecione um veículo válido.", "OK");
                    return;
                }
                veiculoId = motoSelecionado.Id;
            }
            else
            {
                await DisplayAlert("Erro", "Tipo de veículo inválido.", "OK");
                return;
            }

            var disponibilidade = new Disponibilidade
            {
                VeiculoId = veiculoId,
                TipoVeiculo = tipoVeiculo,
                DataInicio = tipoVeiculo == "Carro" ? dateInicioDisponibilidadeCarro.Date : dateInicioDisponibilidadeMoto.Date,
                HoraInicio = tipoVeiculo == "Carro" ? timeInicioDisponibilidadeCarro.Time : timeInicioDisponibilidadeMoto.Time,
                DataFim = tipoVeiculo == "Carro" ? dateFimDisponibilidadeCarro.Date : dateFimDisponibilidadeMoto.Date,
                HoraFim = tipoVeiculo == "Carro" ? timeFimDisponibilidadeCarro.Time : timeFimDisponibilidadeMoto.Time
            };

            await disponibilidadeData.AdicionarDisponibilidadeAsync(disponibilidade);
            await DisplayAlert("Cadastro Realizado", "Disponibilidade cadastrada com sucesso.", "OK");

            ResetDisponibilidadeFields();
        }

        private void ResetDisponibilidadeFields()
        {
            dateInicioDisponibilidadeCarro.Date = DateTime.Today;
            timeInicioDisponibilidadeCarro.Time = DateTime.Now.TimeOfDay;
            dateFimDisponibilidadeCarro.Date = DateTime.Today;
            timeFimDisponibilidadeCarro.Time = DateTime.Now.TimeOfDay;
            dateInicioDisponibilidadeMoto.Date = DateTime.Today;
            timeInicioDisponibilidadeMoto.Time = DateTime.Now.TimeOfDay;
            dateFimDisponibilidadeMoto.Date = DateTime.Today;
            timeFimDisponibilidadeMoto.Time = DateTime.Now.TimeOfDay;
        }
    }

}
