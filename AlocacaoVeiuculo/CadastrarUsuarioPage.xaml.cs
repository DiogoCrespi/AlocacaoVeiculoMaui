namespace AlocacaoVeiuculo
{
    public partial class CadastrarUsuarioPage : ContentPage
    {
        private UsuarioRepositorio usuarioRepositorio;

        public CadastrarUsuarioPage(UsuarioRepositorio repositorio)
        {
            InitializeComponent();
            usuarioRepositorio = repositorio;
        }

        private void OnAcessarButtonClicked(object sender, EventArgs e)
        {
            if (entryNome == null || entrySenha == null)
            {
                DisplayAlert("Erro", "Campos de nome e senha não podem ser nulos.", "OK");
                return;
            }

            string nome = entryNome.Text?.Trim();
            string senha = entrySenha.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
            {
                DisplayAlert("Erro", "Nome ou senha não podem estar vazios.", "OK");
                return;
            }

            if (usuarioRepositorio.UsuarioExistente(nome) && usuarioRepositorio.ObterUsuario(nome).senha == senha)
            {
                var usuario = usuarioRepositorio.ObterUsuario(nome);
                string mensagem = $"Nome: {nome}\nSenha: {usuario.senha}\nCPF: {usuario.cpf}\nData de Nascimento: {usuario.dataNascimento.ToShortDateString()}\nTelefone: {usuario.telefone}";
                DisplayAlert("Usuário Encontrado", mensagem, "OK");
            }
            else
            {
                DisplayAlert("Erro", "Nome ou senha inválidos.", "OK");
                entryNome.Text = string.Empty;
                entrySenha.Text = string.Empty;
                CadastroLayout.IsVisible = true;
            }
        }

        private void OnCadastrarButtonClicked(object sender, EventArgs e)
        {
            if (entryNome == null || entrySenha == null)
            {
                DisplayAlert("Erro", "Campos de nome e senha não podem ser nulos.", "OK");
                return;
            }

            string nome = entryNome.Text?.Trim();
            string senha = entrySenha.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
            {
                DisplayAlert("Erro", "Nome ou senha não podem estar vazios.", "OK");
                CadastroLayout.IsVisible = true;
                return;
            }

            if (usuarioRepositorio.UsuarioExistente(nome))
            {
                DisplayAlert("Erro", "Usuário já cadastrado.", "OK");
                return;
            }

            CadastroLayout.IsVisible = true;
        }

        private void OnSalvarButtonClicked(object sender, EventArgs e)
        {
            string nome = entryNome.Text;
            string senha = entrySenha.Text;
            string cpf = entryCpf.Text;
            DateTime dataNascimento = entryDataNascimento.Date;
            string telefone = entryTelefone.Text;

            usuarioRepositorio.AdicionarUsuario(nome, senha, cpf, dataNascimento, telefone);
            DisplayAlert("Cadastro Realizado", $"Nome: {nome}\nCPF: {cpf}\nData de Nascimento: {dataNascimento.ToShortDateString()}\nTelefone: {telefone}", "OK");

            entryNome.Text = string.Empty;
            entrySenha.Text = string.Empty;
            entryCpf.Text = string.Empty;
            entryDataNascimento.Date = DateTime.Today;
            entryTelefone.Text = string.Empty;

            CadastroLayout.IsVisible = false;
        }
    }
}
