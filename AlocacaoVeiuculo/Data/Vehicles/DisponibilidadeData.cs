using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using AlocacaoVeiuculo.Services;
using AlocacaoVeiuculo.RentalManager.Model.Vehicles;

namespace AlocacaoVeiuculo.Data.Vehicles
{
    public class DisponibilidadeData
    {
        private readonly SQLiteAsyncConnection database;

        public DisponibilidadeData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<int> AdicionarDisponibilidadeAsync(Disponibilidade disponibilidade) => database.InsertAsync(disponibilidade);

        public Task<List<Disponibilidade>> ObterDisponibilidadesAsync() => database.Table<Disponibilidade>().ToListAsync();


        public async Task<List<Disponibilidade>> ObterVeiculosDisponiveisAsync(DateTime dataInicio, TimeSpan horaInicio, DateTime dataFim, TimeSpan horaFim)
        {
            DateTime inicioCompleto = dataInicio.Date.Add(horaInicio);
            DateTime fimCompleto = dataFim.Date.Add(horaFim);

            var disponibilidades = await database.Table<Disponibilidade>()
                .Where(d => d.DataInicio <= fimCompleto && d.DataFim >= inicioCompleto)
                .ToListAsync();

            // Atualiza as informações dos veículos associados
            foreach (var item in disponibilidades)
            {
                if (item.TipoVeiculo == "Carro")
                {
                    var carro = await new CarroData().ObterCarroPorIdAsync(item.VeiculoId);
                    if (carro != null)
                    {
                        item.ImagemPath = carro.ImagemPath;
                        item.Modelo = carro.Modelo;
                        item.TipoCombustivel = carro.TipoCombustivel;
                    }
                }
                else if (item.TipoVeiculo == "Moto")
                {
                    var moto = await new MotoData().ObterMotoPorIdAsync(item.VeiculoId);
                    if (moto != null)
                    {
                        item.ImagemPath = moto.ImagemPath;
                        item.Modelo = moto.Modelo;
                        item.TipoCombustivel = moto.TipoCombustivel;
                    }
                }
            }

            return disponibilidades;
        }

    }
}
