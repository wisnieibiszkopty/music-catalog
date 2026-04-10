using System.Data;

namespace Shared;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}