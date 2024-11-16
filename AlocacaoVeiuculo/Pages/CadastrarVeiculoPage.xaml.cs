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
        private string imagemCarroPath;
        private string imagemMotoPath;

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
            disponibilidadeCarroSection.IsVisible = false;
            disponibilidadeMotoSection.IsVisible = false;

            if (pickerTipoVeiculo.SelectedItem?.ToString() == "Carro")
            {
                carroSection.IsVisible = true;
                motoSection.IsVisible = false;
            }
            else if (pickerTipoVeiculo.SelectedItem?.ToString() == "Moto")
            {
                carroSection.IsVisible = false;
                motoSection.IsVisible = true;
            }
        }

        private async void OnTipoDisponibilidadeVeiculoChanged(object sender, EventArgs e)
        {
            carroSection.IsVisible = false;
            motoSection.IsVisible = false;

            if (pickerDisponibilidadeTipoVeiculo.SelectedItem?.ToString() == "Carro")
            {
                var carros = await carroData.ObterCarrosAsync();
                pickerDisponibilidadeCarro.ItemsSource = carros.Any() ? carros : new List<Carro> { new Carro { Modelo = "Nenhum veículo cadastrado" } };
                pickerDisponibilidadeCarro.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeCarroSection.IsVisible = true;
                disponibilidadeMotoSection.IsVisible = false;
            }
            else if (pickerDisponibilidadeTipoVeiculo.SelectedItem?.ToString() == "Moto")
            {
                var motos = await motoData.ObterMotosAsync();
                pickerDisponibilidadeMoto.ItemsSource = motos.Any() ? motos : new List<Moto> { new Moto { Modelo = "Nenhum veículo cadastrado" } };
                pickerDisponibilidadeMoto.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeMotoSection.IsVisible = true;
                disponibilidadeCarroSection.IsVisible = false;
            }
        }

        private async void OnSelecionarImagemCarroClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione uma imagem para o carro",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imagemCarroPath = result.FullPath;
                    imageCarroPreview.Source = ImageSource.FromFile(imagemCarroPath);
                    imageCarroPreview.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao selecionar imagem: {ex.Message}", "OK");
            }
        }

        private async void OnSelecionarImagemMotoClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione uma imagem para a moto",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    imagemMotoPath = result.FullPath;
                    imageMotoPreview.Source = ImageSource.FromFile(imagemMotoPath);
                    imageMotoPreview.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao selecionar imagem: {ex.Message}", "OK");
            }
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            if (pickerTipoVeiculo.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Selecione o tipo de veículo.", "OK");
                return;
            }

            string tipoVeiculo = pickerTipoVeiculo.SelectedItem.ToString();

            if (tipoVeiculo == "Carro")
            {
                if (string.IsNullOrWhiteSpace(entryPlacaCarro.Text) || string.IsNullOrEmpty(imagemCarroPath))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos e adicione uma imagem.", "OK");
                    return;
                }

                var carro = new Carro
                {
                    Placa = entryPlacaCarro.Text,
                    Modelo = pickerModeloCarro.SelectedItem.ToString(),
                    Ano = (int)pickerAnoCarro.SelectedItem,
                    Quilometragem = double.Parse(entryQuilometragemCarro.Text),
                    TipoCombustivel = pickerTipoCombustivelCarro.SelectedItem.ToString(),
                    NumeroPortas = int.Parse(pickerNumeroPortasCarro.SelectedItem.ToString()),
                    ImagemPath = imagemCarroPath
                };

                await carroData.AdicionarCarroAsync(carro);
                await DisplayAlert("Cadastro Realizado", $"Carro cadastrado: {carro.Modelo} - {carro.Placa}", "OK");
                carroSection.IsVisible = false;
            }
            else if (tipoVeiculo == "Moto")
            {
                if (string.IsNullOrWhiteSpace(entryPlacaMoto.Text) || string.IsNullOrEmpty(imagemMotoPath))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos e adicione uma imagem.", "OK");
                    return;
                }

                var moto = new Moto
                {
                    Placa = entryPlacaMoto.Text,
                    Modelo = pickerModeloMoto.SelectedItem.ToString(),
                    Ano = (int)pickerAnoMoto.SelectedItem,
                    Quilometragem = double.Parse(entryQuilometragemMoto.Text),
                    TipoCombustivel = pickerTipoCombustivelMoto.SelectedItem.ToString(),
                    ImagemPath = imagemMotoPath
                };

                await motoData.AdicionarMotoAsync(moto);
                await DisplayAlert("Cadastro Realizado", $"Moto cadastrada: {moto.Modelo} - {moto.Placa}", "OK");
                motoSection.IsVisible = false;
            }

            ResetFields();
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
            string veiculoDetalhes;

            if (tipoVeiculo == "Carro")
            {
                var carroSelecionado = pickerDisponibilidadeCarro.SelectedItem as Carro;
                if (carroSelecionado == null || carroSelecionado.Modelo == "Nenhum veículo cadastrado")
                {
                    await DisplayAlert("Erro", "Selecione um veículo válido.", "OK");
                    return;
                }
                veiculoId = carroSelecionado.Id;
                veiculoDetalhes = $"Carro: {carroSelecionado.Modelo}\n" +
                                  $"Placa: {carroSelecionado.Placa}\n" +
                                  $"Ano: {carroSelecionado.Ano}\n" +
                                  $"Quilometragem: {carroSelecionado.Quilometragem}\n" +
                                  $"Combustível: {carroSelecionado.TipoCombustivel}\n" +
                                  $"Portas: {carroSelecionado.NumeroPortas}";
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
                veiculoDetalhes = $"Moto: {motoSelecionado.Modelo}\n" +
                                  $"Placa: {motoSelecionado.Placa}\n" +
                                  $"Ano: {motoSelecionado.Ano}\n" +
                                  $"Quilometragem: {motoSelecionado.Quilometragem}\n" +
                                  $"Combustível: {motoSelecionado.TipoCombustivel}";
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

            await DisplayAlert("Cadastro Realizado",
                $"Disponibilidade cadastrada com sucesso para o seguinte veículo:\n\n{veiculoDetalhes}", "OK");


            ResetFields();
            disponibilidadeCarroSection.IsVisible = false;
            disponibilidadeMotoSection.IsVisible = false;
        }

        private void ResetFields()
        {
            entryPlacaCarro.Text = "";
            entryPlacaMoto.Text = "";
            imageCarroPreview.IsVisible = false;
            imageMotoPreview.IsVisible = false;
            imagemCarroPath = null;
            imagemMotoPath = null;
        }
    }
}
