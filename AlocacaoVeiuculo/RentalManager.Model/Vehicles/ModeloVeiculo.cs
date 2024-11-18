using SQLite;

namespace AlocacaoVeiuculo.RentalManager.Model.Vehicles
{
    public class ModeloVeiculo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; } // "Carro" ou "Moto"
    }
}
