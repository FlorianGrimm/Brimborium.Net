#if true
using System;

namespace Brimborium.TestSample.Record {
    public sealed record ActivityPK (
        DateTimeOffset CreatedAt,
        Guid Id
        ) : IPrimaryKey;

    public sealed record ProjectPK (
        Guid Id
        ) : IPrimaryKey;

    public sealed record ToDoPK (
        Guid Id
        ) : IPrimaryKey;

    public sealed record UserPK (
        Guid Id
        ) : IPrimaryKey;

    public sealed record ProjectHistoryPK (
        DateTimeOffset ValidTo,
        DateTimeOffset ValidFrom,
        Guid ActivityId,
        Guid Id
        ) : IPrimaryKey;

    public sealed record ToDoHistoryPK (
        DateTimeOffset ValidTo,
        DateTimeOffset ValidFrom,
        Guid ActivityId,
        Guid Id
        ) : IPrimaryKey;

    public sealed record UserHistoryPK (
        DateTimeOffset ValidTo,
        DateTimeOffset ValidFrom,
        Guid ActivityId,
        Guid Id
        ) : IPrimaryKey;
}

#endif
