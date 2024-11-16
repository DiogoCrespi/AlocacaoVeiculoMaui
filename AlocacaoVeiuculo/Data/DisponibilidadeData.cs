using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlocacaoVeiuculo.Modelo;
using SQLite;
using AlocacaoVeiuculo.Services;

namespace AlocacaoVeiuculo.Data
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

            // Atualiza as imagens dos veículos associados
            foreach (var item in disponibilidades)
            {
                if (item.TipoVeiculo == "Carro")
                {
                    var carro = await new CarroData().ObterCarroPorIdAsync(item.VeiculoId);
                    item.ImagemPath = carro?.ImagemPath;
                }
                else if (item.TipoVeiculo == "Moto")
                {
                    var moto = await new MotoData().ObterMotoPorIdAsync(item.VeiculoId);
                    item.ImagemPath = moto?.ImagemPath;
                }
            }

            return disponibilidades;
        }
    }
}
