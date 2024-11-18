using AlocacaoVeiuculo.RentalManager.Model.Vehicles;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data.Vehicles
{
    public class ModeloVeiculoData
    {
        private readonly SQLiteAsyncConnection database;

        public ModeloVeiculoData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<List<ModeloVeiculo>> ObterModelosAsync() => database.Table<ModeloVeiculo>().ToListAsync();

        public Task<int> AdicionarModeloAsync(ModeloVeiculo modelo) => database.InsertAsync(modelo);

        public async Task InicializarDados()
        {
            var modelosExistentes = await ObterModelosAsync();
            if (modelosExistentes.Count == 0)
            {
                var modelosCarro = new List<ModeloVeiculo>
                {
                    new ModeloVeiculo { Nome = "Corolla", Tipo = "Carro" },
                    new ModeloVeiculo { Nome = "Civic", Tipo = "Carro" },
                    new ModeloVeiculo { Nome = "Gol", Tipo = "Carro" },
                    new ModeloVeiculo { Nome = "Onix", Tipo = "Carro" },
                    new ModeloVeiculo { Nome = "Fiesta", Tipo = "Carro" }
                };

                var modelosMoto = new List<ModeloVeiculo>
                {
                    new ModeloVeiculo { Nome = "CB 500", Tipo = "Moto" },
                    new ModeloVeiculo { Nome = "XJ6", Tipo = "Moto" },
                    new ModeloVeiculo { Nome = "CG 160", Tipo = "Moto" },
                    new ModeloVeiculo { Nome = "MT-07", Tipo = "Moto" },
                    new ModeloVeiculo { Nome = "Ninja 400", Tipo = "Moto" }
                };

                foreach (var modelo in modelosCarro) await AdicionarModeloAsync(modelo);
                foreach (var modelo in modelosMoto) await AdicionarModeloAsync(modelo);
            }
        }
    }
}
