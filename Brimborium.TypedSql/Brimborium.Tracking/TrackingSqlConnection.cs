﻿using System;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Common;

namespace Brimborium.Tracking;

public class TrackingSqlConnectionOption {
    public string? ConnectionString { get; set; }
}

public sealed class TrackingSqlConnection
    : TrackingConnection {

    public TrackingSqlConnection() {
        this.ConnectionString = String.Empty;
    }

    public TrackingSqlConnection(IOptions<TrackingSqlConnectionOption> options) {
        if (string.IsNullOrEmpty(options.Value.ConnectionString)) {
            this.ConnectionString = String.Empty;
        } else {
            this.ConnectionString = options.Value.ConnectionString;
        }
    }

    public string ConnectionString { get; set; }

    public override TrackingTransaction BeginTransaction() {
        if (string.IsNullOrEmpty(this.ConnectionString)) {
            throw new System.InvalidOperationException("ConnectionString is empty.");
        }
        return new TrackingSqlTransaction(this.ConnectionString);
    }
}

public sealed class TrackingSqlTransaction
    : TrackingTransaction
    , IDisposable {
    private readonly string _ConnectionString;
    private SqlConnection? _Connection;
    private DbTransaction? _Transaction;

    internal TrackingSqlTransaction(string connectionString) {
        this._ConnectionString = connectionString;
        System.GC.SuppressFinalize(this);
    }

    public async Task<(SqlConnection connection, DbTransaction transaction)> OpenAsync(CancellationToken cancellationToken = default(CancellationToken)) {
        if ((this._Connection is null) || (_Transaction is null)) {
            this._Connection = new SqlConnection(this._ConnectionString);
            await this._Connection.OpenAsync();
            this._Transaction = await this._Connection.BeginTransactionAsync(System.Data.IsolationLevel.Unspecified, cancellationToken);
            System.GC.ReRegisterForFinalize(this);
        }
        return (this._Connection, this._Transaction);
    }

    public override async Task CommitAsync() {
        if (this._Connection is null) {
            throw new System.InvalidOperationException("no connection");
        } else if (this._Transaction is null) {
            throw new System.InvalidOperationException("no transaction");
        } else {
            await this._Transaction.CommitAsync();
            await this._Transaction.DisposeAsync();
            await this._Connection.CloseAsync();
            this._Transaction = null;
            this._Connection = null;
            System.GC.SuppressFinalize(this);
        }
    }


    protected override bool Dispose(bool disposing) {
        var result = base.Dispose(disposing);
        if (result) {
            using (var c = this._Connection) {
                using (var t = this._Transaction) {
                    this._Transaction = null;
                    this._Connection = null;
                }
            }
        }
        return result;
    }
}