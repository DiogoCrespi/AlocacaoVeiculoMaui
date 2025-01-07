using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace AlocacaoVeiuculo.Pages
{
    public partial class MapaPage : ContentPage
    {
        public event EventHandler<string> LocalSelecionado;

        public MapaPage()
        {
            InitializeComponent();
            CarregarMapa();
        }

        private void CarregarMapa()
        {
            string mapFilePath = Path.Combine(AppContext.BaseDirectory, "Resources\\Raw\\map.html");

            if (!File.Exists(mapFilePath))
            {
                throw new FileNotFoundException("O arquivo map.html não foi encontrado no diretório de saída.");
            }

            webViewMapa.Source = new UrlWebViewSource
            {
                Url = $"file:///{mapFilePath.Replace("\\", "/")}"
            };
        }

        private async void OnConfirmarLocalClicked(object sender, EventArgs e)
        {
            try
            {
                // Obter o endereço selecionado
                var resultado = await webViewMapa.EvaluateJavaScriptAsync("JSON.stringify(window.selectedAddress)");
                if (!string.IsNullOrWhiteSpace(resultado) && resultado != "null")
                {
                    LocalSelecionado?.Invoke(this, resultado.Trim('"')); // Remover aspas do JSON
                    await DisplayAlert("Local Selecionado", $"Endereço: {resultado.Trim('"')}", "OK");

                    // Navega de volta para a página inicial (MainPage)
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Nenhum endereço foi encontrado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao confirmar local: {ex.Message}", "OK");
            }
        }

    }
}
