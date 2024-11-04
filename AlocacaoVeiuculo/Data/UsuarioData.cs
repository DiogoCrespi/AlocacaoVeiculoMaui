using AlocacaoVeiuculo.Modelo;
using AlocacaoVeiuculo.Services;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlocacaoVeiuculo.Data
{
    public class UsuarioData
    {
        private readonly SQLiteAsyncConnection database;

        public UsuarioData()
        {
            database = DatabaseService.GetDatabaseAsync().Result;
        }

        public Task<List<Usuario>> ObterUsuariosAsync() => database.Table<Usuario>().ToListAsync();

        public Task<int> AdicionarUsuarioAsync(Usuario usuario) => database.InsertAsync(usuario);

        public Task<int> AtualizarUsuarioAsync(Usuario usuario) => database.UpdateAsync(usuario);

        public Task<int> RemoverUsuarioAsync(Usuario usuario) => database.DeleteAsync(usuario);
    }
}
