
using System;
using System.Data;

namespace Brimborium.SqlAccess;

public interface ISqlAccessBase : IDisposable {
    IDisposable Connected();

    IDbTransaction BeginTransaction();

    public IDbConnection Connection { get; }
}
