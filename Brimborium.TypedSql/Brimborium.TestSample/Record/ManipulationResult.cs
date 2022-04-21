namespace Brimborium.TestSample.Record;

public sealed record ProjectManipulationResult(
        Project DataResult,
        OperationResult OperationResult
    ) : IDataManipulationResult<Project>;

public sealed record UserManipulationResult(
        User DataResult,
        OperationResult OperationResult
    ) : IDataManipulationResult<User>;

public sealed record ToDoManipulationResult(
        ToDo DataResult,
        OperationResult OperationResult
    ) : IDataManipulationResult<ToDo>;
