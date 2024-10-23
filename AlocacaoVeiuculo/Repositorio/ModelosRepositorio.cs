namespace AlocacaoVeiuculo
{
    public class ModeloRepositorio
    {
        public List<string> ObterModelosCarro()
        {
            return new List<string>
            {
                "Corolla",
                "Civic",
                "Gol",
                "Onix",
                "Fiesta"
                // Adicione mais modelos de carros
            };
        }

        public List<string> ObterModelosMoto()
        {
            return new List<string>
            {
                "CB 500",
                "XJ6",
                "CG 160",
                "MT-07",
                "Ninja 400"
                // Adicione mais modelos de motos
            };
        }
    }
}
