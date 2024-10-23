namespace AlocacaoVeiuculo
{
    public class VeiculoRepositorio
    {
        private List<Carro> carros = new();
        private List<Moto> motos = new();

        public void AdicionarCarro(Carro carro)
        {
            carros.Add(carro);
        }

        public void AdicionarMoto(Moto moto)
        {
            motos.Add(moto);
        }

        public List<Carro> ObterCarros()
        {
            return carros;
        }

        public List<Moto> ObterMotos()
        {
            return motos;
        }
    }
}
