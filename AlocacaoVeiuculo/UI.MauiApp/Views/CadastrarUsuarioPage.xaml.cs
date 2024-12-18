using AlocacaoVeiuculo.RentalManager.Model;
using AlocacaoVeiuculo.RentalManager.Model.Users;
using AlocacaoVeiuculo.RentalManager.Model.Reservations;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using AlocacaoVeiuculo.Data.User;

namespace AlocacaoVeiuculo
{
    public partial class CadastrarUsuarioPage : ContentPage
    {
        private UsuarioData usuarioData;
        private MainPage mainPage;

        public CadastrarUsuarioPage(MainPage mainPage)
        {
            InitializeComponent();
            usuarioData = new UsuarioData();
            this.mainPage = mainPage;
        }

        private async void OnAcessarButtonClicked(object sender, EventArgs e)
        {
            string nome = entryNome?.Text?.Trim();
            string senha = entrySenha?.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
            {
                await DisplayAlert("Erro", "Nome ou senha não podem estar vazios.", "OK");
                return;
            }

            var usuario = await usuarioData.ObterUsuarioPorNomeAsync(nome);
            if (usuario != null && usuario.Senha == senha)
            {
                if (!usuario.IsDisponivel)
                {
                    // Mostrar o motivo da exclusão e reativar o usuário
                    await DisplayAlert("Conta Desativada", $"Motivo da exclusão: {usuario.MotivoExclusao}", "OK");
                    usuario.IsDisponivel = true;
                    usuario.MotivoExclusao = null;
                    await usuarioData.AtualizarUsuarioAsync(usuario);
                    await DisplayAlert("Conta Reativada", "Sua conta foi reativada com sucesso. Bem-vindo de volta!", "OK");
                }

                string mensagem = $"Nome: {nome}\nSenha: {usuario.Senha}\nCPF: {usuario.Cpf}\nData de Nascimento: {usuario.DataNascimento.ToShortDateString()}\nTelefone: {usuario.Telefone}";
                await DisplayAlert("Usuário Encontrado", mensagem, "OK");

                mainPage.MostrarBotaoUsuarioLogado(usuario);
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", "Nome ou senha inválidos.", "OK");
                CadastroLayout.IsVisible = true;
            }
        }
        private void OnSenhaCompleted(object sender, EventArgs e)
        {
            OnAcessarButtonClicked(sender, e);
        }

        private async void OnCadastrarButtonClicked(object sender, EventArgs e)
        {
            string nome = entryNome?.Text?.Trim();
            string senha = entrySenha?.Text?.Trim();

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
            {
                await DisplayAlert("Erro", "Nome ou senha não podem estar vazios.", "OK");
                CadastroLayout.IsVisible = true;
                return;
            }

            if (await usuarioData.UsuarioExistenteAsync(nome))
            {
                await DisplayAlert("Erro", "Usuário já cadastrado.", "OK");
                return;
            }

            CadastroLayout.IsVisible = true;
        }


        private async void OnSalvarButtonClicked(object sender, EventArgs e)
        {
            string nome = entryNome.Text;
            string senha = entrySenha.Text;
            string cpf = entryCpf.Text;
            DateTime dataNascimento = entryDataNascimento.Date;
            string telefone = entryTelefone.Text;

            var novoUsuario = new Usuario
            {
                Nome = nome,
                Senha = senha,
                Cpf = cpf,
                DataNascimento = dataNascimento,
                Telefone = telefone,
                IsDisponivel = true // Garante que o usuário seja ativado ao ser criado
            };

            await usuarioData.AdicionarUsuarioAsync(novoUsuario);
            await DisplayAlert("Cadastro Realizado", $"Nome: {nome}\nCPF: {cpf}\nData de Nascimento: {dataNascimento.ToShortDateString()}\nTelefone: {telefone}", "OK");

            mainPage.MostrarBotaoUsuarioLogado(novoUsuario); // Loga o usuário automaticamente

            await Navigation.PopAsync(); // Retorna à MainPage após o cadastro
        }
    }
}
