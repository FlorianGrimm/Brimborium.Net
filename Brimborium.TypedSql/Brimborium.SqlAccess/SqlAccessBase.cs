
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Brimborium.SqlAccess;
public class SqlAccessBase : IDisposable, ISqlAccessBase {
    private Microsoft.Data.SqlClient.SqlConnection? _SQLConnection;
    protected Microsoft.Data.SqlClient.SqlConnection SQLConnection => this._SQLConnection ?? throw new ObjectDisposedException(nameof(SQLConnection));
    protected IDbConnection SQLDBConnection => this._SQLConnection ?? throw new ObjectDisposedException(nameof(SQLDBConnection));

    public IDbConnection Connection => this._SQLConnection ?? throw new ObjectDisposedException(nameof(Connection));

    private bool _OwnsConnection;

    protected SqlAccessBase(IDbConnection connection) {
        this._SQLConnection = (Microsoft.Data.SqlClient.SqlConnection)connection;
        this._OwnsConnection = false;
    }

    protected SqlAccessBase(string connectionString) {
        this._SQLConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        this._OwnsConnection = true;
    }

    public IDisposable Connected() => DbConnectionExtensions.Connected(this.SQLDBConnection);

    public IDbTransaction BeginTransaction() => this.BeginTransaction();

    public Microsoft.Data.SqlClient.SqlCommand CreateCommand(
        string commandText,
        CommandType commandType,
        IDbTransaction? tx) {
        var result = this.SQLConnection.CreateCommand();
        result.CommandType = commandType;
        result.CommandText = commandText;
        result.Transaction = tx as Microsoft.Data.SqlClient.SqlTransaction;
        return result;
    }

    protected void AddParameterBoolean(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, bool? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.Bit) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterDateTime(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, SqlDbType sqlDbType, DateTime? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, sqlDbType) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterDateTimeOffset(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, DateTimeOffset? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.DateTimeOffset) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterByte(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, byte? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.TinyInt) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterShort(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, short? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.SmallInt) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterInt(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, int? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.Int) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterLong(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, long? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.BigInt) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterGuid(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, Guid? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.UniqueIdentifier) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterString(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, SqlDbType sqlDbType, int size, string? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, sqlDbType, size) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterFloat(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, SqlDbType sqlDbType, float? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, sqlDbType) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterDouble(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, SqlDbType sqlDbType, double? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, sqlDbType) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterDecimal(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, SqlDbType sqlDbType, decimal? value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, sqlDbType) { Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterVarBinaryMax(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, byte[] value) {
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.VarBinary) { Size = -1, Value = (value == null) ? DBNull.Value : value });
    }

    protected void AddParameterStructured<T>(Microsoft.Data.SqlClient.SqlCommand command, string parameterName, T value,
        Func<T, List<Microsoft.Data.SqlClient.Server.SqlDataRecord>> converter) {
        var parameter = new Microsoft.Data.SqlClient.SqlParameter(parameterName, SqlDbType.Structured);
        command.Parameters.Add(parameter);
    }


    protected int CommandExecuteNonQuery(
        Microsoft.Data.SqlClient.SqlCommand cmd
        ) {
        return cmd.ExecuteNonQuery();
    }

    protected async Task<int> CommandExecuteNonQueryAsync(
        Microsoft.Data.SqlClient.SqlCommand cmd
        ) {
        return await cmd.ExecuteNonQueryAsync();
    }

    protected T CommandExecuteScalar<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd
        ) {
        return (T)cmd.ExecuteScalar();
    }

    protected async Task<T?> CommandExecuteScalarAsync<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd
        ) {
        return (T?)(await cmd.ExecuteScalarAsync());
    }
    protected T CommandQuerySingle<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
        ) {
        var result = new List<T>();
        using (Microsoft.Data.SqlClient.SqlDataReader? reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result.Single();
            }
        }
    }

    protected async Task<T> CommandQuerySingleAsync<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
        ) {
        var result = new List<T>();
        using (Microsoft.Data.SqlClient.SqlDataReader? reader = cmd.ExecuteReader()) {
            while (await reader.ReadAsync()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result.Single();
            }
        }
    }

    protected T? CommandQuerySingleOrDefault<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
        ) {
        var result = new List<T>();
        using (Microsoft.Data.SqlClient.SqlDataReader? reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result.SingleOrDefault();
            }
        }
    }

    protected async Task<T?> CommandQuerySingleOrDefaultAsync<T>(
       Microsoft.Data.SqlClient.SqlCommand cmd,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        using (var reader = await cmd.ExecuteReaderAsync()) {
            while (reader.Read()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result.SingleOrDefault();
            }
        }
    }

    protected List<T> CommandQuery<T>(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
        ) {
        var result = new List<T>();
        using (var reader = cmd.ExecuteReader()) {
            while (reader.Read()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result;
            }
        }
    }

    protected async Task<List<T>> CommandQueryAsync<T>(
       Microsoft.Data.SqlClient.SqlCommand cmd,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        using (var reader = await cmd.ExecuteReaderAsync()) {
            while (reader.Read()) {
                result.Add(readRecord(reader));
            }
            if (reader.NextResult()) {
                throw new InvalidOperationException("unexpected 2cd result set.");
            } else {
                return result;
            }
        }
    }

    protected void CommandQueryMultiple(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Action<int, Microsoft.Data.SqlClient.SqlDataReader> readResultSet,
        int cntResultSet
        ) {
        using (var reader = cmd.ExecuteReader()) {
            int idx = 0;
            do {
                readResultSet(idx, reader);
                idx++;
                //while (reader.Read()) {
                //    result.Add(readRecord(reader));
                //}
            } while (reader.NextResult());
            if ((cntResultSet == 0) || (cntResultSet == idx)) {
                return;
            } else {
                throw new InvalidOperationException($"#ResultSets {idx} actual, but #ResultSets {cntResultSet} expected");
            }
        }
    }

    protected async Task CommandQueryMultipleAsync(
        Microsoft.Data.SqlClient.SqlCommand cmd,
        Func<int, Microsoft.Data.SqlClient.SqlDataReader, Task> readResultSetAsync,
        int cntResultSet
        ) {
        using (var reader = await cmd.ExecuteReaderAsync()) {
            int idx = 0;
            do {
                await readResultSetAsync(idx, reader);
                idx++;
            } while (await reader.NextResultAsync());
            if ((cntResultSet == 0) || (cntResultSet == idx)) {
                return;
            } else {
                throw new InvalidOperationException($"#ResultSets {idx} actual, but #ResultSets {cntResultSet} expected");
            }
        }
    }

    protected List<T> CommandReadQuery<T>(
       Microsoft.Data.SqlClient.SqlDataReader reader,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        while (reader.Read()) {
            result.Add(readRecord(reader));
        }
        return result;
    }

    protected async Task<List<T>> CommandReadQueryAsync<T>(
       Microsoft.Data.SqlClient.SqlDataReader reader,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        while (await reader.ReadAsync()) {
            result.Add(readRecord(reader));
        }
        return result;
    }

    protected T CommandReadScalar<T>(
       Microsoft.Data.SqlClient.SqlDataReader reader
       ) {
        var result = new List<T>();
        while (reader.Read()) {
            var value = reader.GetValue(0);
            if (value == DBNull.Value) {
                result.Add((T)default!);
            } else {
                result.Add((T)value);
            }
        }
        return result.Single();
    }

    protected T CommandReadQuerySingle<T>(
      Microsoft.Data.SqlClient.SqlDataReader reader,
      Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
      ) {
        var result = new List<T>();
        while (reader.Read()) {
            result.Add(readRecord(reader));
        }
        return result.Single();
    }

    protected async Task<T> CommandReadQuerySingleAsync<T>(
      Microsoft.Data.SqlClient.SqlDataReader reader,
      Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
      ) {
        var result = new List<T>();
        while (await reader.ReadAsync()) {
            result.Add(readRecord(reader));
        }
        return result.Single();
    }

    protected T? CommandReadQuerySingleOrDefault<T>(
       Microsoft.Data.SqlClient.SqlDataReader reader,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        while (reader.Read()) {
            result.Add(readRecord(reader));
        }
        return result.SingleOrDefault();
    }

    protected async Task<T?> CommandReadQuerySingleOrDefaultAsync<T>(
       Microsoft.Data.SqlClient.SqlDataReader reader,
       Func<Microsoft.Data.SqlClient.SqlDataReader, T> readRecord
       ) {
        var result = new List<T>();
        while (await reader.ReadAsync()) {
            result.Add(readRecord(reader));
        }
        return result.SingleOrDefault();
    }


    protected byte[] ReadByteArray(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to byte[]") : reader.GetFieldValue<byte[]>(index);

    protected byte[]? ReadByteArrayQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetFieldValue<byte[]>(index);

    protected bool ReadBoolean(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to bool") : reader.GetBoolean(index);

    protected bool? ReadBooleanQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetBoolean(index);

    protected DateTime ReadDateTime(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to DateTime") : reader.GetDateTime(index);

    protected DateTime? ReadDateTimeQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetDateTime(index);

    protected DateTimeOffset ReadDateTimeOffset(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to DateTime") : reader.GetDateTimeOffset(index);

    protected DateTimeOffset? ReadDateTimeOffsetQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetDateTimeOffset(index);

    protected short ReadInt16(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
       => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to int") : reader.GetInt16(index);

    protected short? ReadInt16Q(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetInt16(index);

    protected int ReadInt32(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to int") : reader.GetInt32(index);

    protected int? ReadInt32Q(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetInt32(index);

    protected long ReadInt64(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to int") : reader.GetInt64(index);

    protected long? ReadInt64Q(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetInt64(index);

    protected float ReadFloat(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to int") : reader.GetFloat(index);

    protected float? ReadFloatQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetFloat(index);

    protected double ReadDouble(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to int") : reader.GetDouble(index);

    protected double? ReadDoubleQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetDouble(index);

    protected Guid ReadGuid(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException("DBNULL to Guid") : reader.GetGuid(index);

    protected Guid? ReadGuidQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetGuid(index);

    protected string ReadString(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? "" : reader.GetFieldValue<string>(index);

    protected string? ReadStringQ(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? null : reader.GetFieldValue<string>(index);

    protected T ReadValue<T>(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        => reader.IsDBNull(index) ? throw new InvalidCastException($"DBNULL to {typeof(T).FullName}") : reader.GetFieldValue<T>(index);

    protected Nullable<T> ReadValueQ<T>(Microsoft.Data.SqlClient.SqlDataReader reader, int index)
        where T : struct
        => reader.IsDBNull(index) ? ((Nullable<T>)null) : ((Nullable<T>)reader.GetFieldValue<T>(index));

    protected virtual void Dispose(bool disposing) {
        if (this._OwnsConnection) {
            if (disposing) {
                using (var db = this._SQLConnection) {
                    this._SQLConnection = null;
                }
            } else {
                try {
                    using (var db = this._SQLConnection) {
                        this._SQLConnection = null;
                    }
                } catch {
                    // since this is the destructor finalizer thread a exception will be thrown.
                }
            }
        } else {
            this._SQLConnection = null;
        }
    }

    ~SqlAccessBase() {
        this.Dispose(false);
    }

    public void Dispose() {
        this.Dispose(true);
        System.GC.SuppressFinalize(this);
    }
}