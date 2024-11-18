using AlocacaoVeiuculo.Data.Vehicles;
using AlocacaoVeiuculo.Services;

namespace AlocacaoVeiuculo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
        InicializarBancoDeDadosAsync();
    }

    private async void InicializarBancoDeDadosAsync()
    {
        await DatabaseService.GetDatabaseAsync();
        await new ModeloVeiculoData().InicializarDados();
    }
}
