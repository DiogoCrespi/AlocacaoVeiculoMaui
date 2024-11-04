using SQLite;
using AlocacaoVeiuculo.Data;
using System.IO;

namespace AlocacaoVeiuculo
{
    public class SQLiteData : ISQLiteBD
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AlocacaoVeiculo.db3");
            return new SQLiteAsyncConnection(dbPath);
        }
    }
}
