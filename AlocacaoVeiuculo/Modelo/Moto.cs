using SQLite;

namespace AlocacaoVeiuculo.Modelo
{
    public class Moto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public double Quilometragem { get; set; }
        public string TipoCombustivel { get; set; }
        public string ImagemPath { get; set; } // Caminho da imagem
    }
}
