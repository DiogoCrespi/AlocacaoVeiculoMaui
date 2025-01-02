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
           // InitializeComponent();
            CarregarMapa();
        }

        private void CarregarMapa()
        {
            string mapFilePath = Path.Combine(FileSystem.AppDataDirectory, "map.html");

            if (!File.Exists(mapFilePath))
            {
                var assembly = typeof(MapaPage).Assembly;
                using var stream = assembly.GetManifestResourceStream("AlocacaoVeiuculo.Resources.Raw.map.html");
                using var fileStream = File.Create(mapFilePath);
                stream.CopyTo(fileStream);
            }

           // webViewMapa.Source = new Uri(mapFilePath);
        }

        private void OnConfirmarLocalClicked(object sender, EventArgs e)
        {
            string localSelecionado = "Latitude, Longitude"; // Substituir com lógica real
            LocalSelecionado?.Invoke(this, localSelecionado);
            Navigation.PopAsync();
        }
    }
}
