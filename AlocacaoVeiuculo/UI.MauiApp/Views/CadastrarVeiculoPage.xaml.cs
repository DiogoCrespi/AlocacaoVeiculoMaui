using AlocacaoVeiuculo.RentalManager.Model;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.Data.Vehicles;

namespace AlocacaoVeiuculo
{
    public partial class CadastrarVeiculoPage : ContentPage
    {
        private CarroData carroData;
        private MotoData motoData;
        private ModeloVeiculoData modeloVeiculoData;
        private DisponibilidadeData disponibilidadeData;
        private string tipoVeiculoSelecionado;
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



        private void OnCarroButtonClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Carro"; // Definir o tipo como "Carro"
            carroSection.IsVisible = true;     // Exibe a seção de carro
            motoSection.IsVisible = false;     // Esconde a seção de moto
        }

        private void OnMotoButtonClicked(object sender, EventArgs e)
        {
            tipoVeiculoSelecionado = "Moto";  // Definir o tipo como "Moto"
            carroSection.IsVisible = false;    // Esconde a seção de carro
            motoSection.IsVisible = true;      // Exibe a seção de moto
        }


        private async void OnDisponibilidadeCarroClicked(object sender, EventArgs e)
        {
            disponibilidadeCarroSection.IsVisible = false;
            disponibilidadeMotoSection.IsVisible = false;

            var carros = await carroData.ObterCarrosAsync();

            var carrosDisponiveis = carros.Where(carro => !carro.IsAlugado).ToList();

            if (carrosDisponiveis.Any())
            {
                pickerDisponibilidadeCarro.ItemsSource = carrosDisponiveis;
                pickerDisponibilidadeCarro.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeCarroSection.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Atenção", "Nenhum carro disponível para disponibilização.", "OK");
            }
        }

        private async void OnDisponibilidadeMotoClicked(object sender, EventArgs e)
        {
            disponibilidadeCarroSection.IsVisible = false;
            disponibilidadeMotoSection.IsVisible = false;

            var motos = await motoData.ObterMotosAsync();

            var motosDisponiveis = motos.Where(moto => !moto.IsAlugado).ToList();

            if (motosDisponiveis.Any())
            {
                pickerDisponibilidadeMoto.ItemsSource = motosDisponiveis;
                pickerDisponibilidadeMoto.ItemDisplayBinding = new Binding("Modelo");
                disponibilidadeMotoSection.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Atenção", "Nenhuma moto disponível para disponibilização.", "OK");
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
            if (string.IsNullOrEmpty(tipoVeiculoSelecionado))
            {
                await DisplayAlert("Erro", "Selecione o tipo de veículo.", "OK");
                return;
            }

            if (tipoVeiculoSelecionado == "Carro")
            {
                if (string.IsNullOrWhiteSpace(entryPlacaCarro.Text) ||
                    pickerModeloCarro.SelectedItem == null ||
                    pickerAnoCarro.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(entryQuilometragemCarro.Text) ||
                    pickerTipoCombustivelCarro.SelectedItem == null ||
                    pickerNumeroPortasCarro.SelectedItem == null ||
                    string.IsNullOrEmpty(imagemCarroPath))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos corretamente e adicione uma imagem.", "OK");
                    return;
                }

                try
                {
                    var carro = new Carro
                    {
                        Placa = entryPlacaCarro.Text,
                        Modelo = pickerModeloCarro.SelectedItem.ToString(),
                        Ano = (int)pickerAnoCarro.SelectedItem,
                        Quilometragem = double.Parse(entryQuilometragemCarro.Text),
                        TipoCombustivel = pickerTipoCombustivelCarro.SelectedItem.ToString(),
                        NumeroPortas = int.Parse(pickerNumeroPortasCarro.SelectedItem.ToString()),
                        ImagemPath = imagemCarroPath,
                        IsAlugado = false
                    };

                    await carroData.AdicionarCarroAsync(carro);
                    await DisplayAlert("Cadastro Realizado", $"Carro cadastrado: {carro.Modelo} - {carro.Placa}", "OK");
                    carroSection.IsVisible = false;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao salvar o carro: {ex.Message}", "OK");
                }
            }
            else if (tipoVeiculoSelecionado == "Moto")
            {
                if (string.IsNullOrWhiteSpace(entryPlacaMoto.Text) ||
                    pickerModeloMoto.SelectedItem == null ||
                    pickerAnoMoto.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(entryQuilometragemMoto.Text) ||
                    pickerTipoCombustivelMoto.SelectedItem == null ||
                    string.IsNullOrEmpty(imagemMotoPath))
                {
                    await DisplayAlert("Erro", "Preencha todos os campos corretamente e adicione uma imagem.", "OK");
                    return;
                }

                try
                {
                    var moto = new Moto
                    {
                        Placa = entryPlacaMoto.Text,
                        Modelo = pickerModeloMoto.SelectedItem.ToString(),
                        Ano = (int)pickerAnoMoto.SelectedItem,
                        Quilometragem = double.Parse(entryQuilometragemMoto.Text),
                        TipoCombustivel = pickerTipoCombustivelMoto.SelectedItem.ToString(),
                        ImagemPath = imagemMotoPath,
                        IsAlugado = false
                    };

                    await motoData.AdicionarMotoAsync(moto);
                    await DisplayAlert("Cadastro Realizado", $"Moto cadastrada: {moto.Modelo} - {moto.Placa}", "OK");
                    motoSection.IsVisible = false;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao salvar a moto: {ex.Message}", "OK");
                }
            }

            ResetFields();
        }



        private async void OnSalvarDisponibilidadeClicked(object sender, EventArgs e)
        {
            string tipoVeiculo = disponibilidadeCarroSection.IsVisible ? "Carro" : "Moto";
            int veiculoId;
            string veiculoDetalhes;

            if (tipoVeiculo == "Carro")
            {
                var carroSelecionado = pickerDisponibilidadeCarro.SelectedItem as Carro;
                if (carroSelecionado == null || carroSelecionado.IsAlugado)
                {
                    await DisplayAlert("Erro", "Este veículo já está alugado ou não selecionado.", "OK");
                    return;
                }

                veiculoId = carroSelecionado.Id;
                veiculoDetalhes = $"Carro: {carroSelecionado.Modelo}\n" +
                                  $"Placa: {carroSelecionado.Placa}\n" +
                                  $"Ano: {carroSelecionado.Ano}";

                carroSelecionado.IsAlugado = false;
                await carroData.AtualizarCarroAsync(carroSelecionado);
            }
            else if (tipoVeiculo == "Moto")
            {
                var motoSelecionada = pickerDisponibilidadeMoto.SelectedItem as Moto;
                if (motoSelecionada == null || motoSelecionada.IsAlugado)
                {
                    await DisplayAlert("Erro", "Este veículo já está alugado ou não selecionado.", "OK");
                    return;
                }

                veiculoId = motoSelecionada.Id;
                veiculoDetalhes = $"Moto: {motoSelecionada.Modelo}\n" +
                                  $"Placa: {motoSelecionada.Placa}\n" +
                                  $"Ano: {motoSelecionada.Ano}";

                motoSelecionada.IsAlugado = false;
                await motoData.AtualizarMotoAsync(motoSelecionada);
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
