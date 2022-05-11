using Brimborium.TestSample.Record;

using Brimborium.TypedStoredProcedure;

namespace Brimborium.TestTypedStoredProcedure;

public static partial class Program {
    private static void AddNativeTypeConverter() {
        SQLUtility.AddDefaultTypeConverter();
        //SQLUtility.AddTypeConverter(
        //    typeof(int),
        //    typeof(bool),
        //    typeof(Replacement.Services.SqlHelpers.Int32Converter),
        //    nameof(Replacement.Services.SqlHelpers.Int32Converter.ToBool));
    }

    private static CSTypeDefinition TypeOfTManipulationResult<TManipulationResult, TRecord>() {
        return CSTypeDefinition.TypeOf<TManipulationResult>(
            members: new MemberDefinition[] {
            new MemberDefinition(
                nameof(IDataManipulationResult<TRecord>.DataResult),
                CSTypeDefinition.TypeOf<TRecord>(
                    isAsync:true,
                    isList:false)
                ),
            new MemberDefinition(
                nameof(IDataManipulationResult<TRecord>.OperationResult),
                CSTypeDefinition.TypeOf<OperationResult>(
                    isAsync:true,
                    isList:false)
                )
            },
            isAsync: true);
    }
    public static DatabaseDefintion GetDefintion() {
        return new DatabaseDefintion(
             StoredProcedures: new StoredProcedureDefintion[] {
                new StoredProcedureDefintion("dbo", "ActivityInsert",
                    CSTypeDefinition.TypeOf<Activity>(),
                    ExecutionMode.QuerySingle,
                    CSTypeDefinition.TypeOf<Activity>(isList:false, isAsync: true)),

                new StoredProcedureDefintion("dbo", "ActivitySelectPK",
                    CSTypeDefinition.TypeOf<ActivityPK>(),
                    ExecutionMode.QuerySingleOrDefault,
                    CSTypeDefinition.TypeOf<Activity>(isList:false, isAsync: true)),

                new StoredProcedureDefintion("dbo", "ProjectDeletePK", 
                    CSTypeDefinition.TypeOf<Project>(), 
                    ExecutionMode.Query, 
                    CSTypeDefinition.TypeOf<ProjectPK>(isList:true, isAsync:true)),
                new StoredProcedureDefintion("dbo", "ProjectSelectPK",
                    CSTypeDefinition.TypeOf<ProjectPK>(), 
                    ExecutionMode.QuerySingleOrDefault,
                    CSTypeDefinition.TypeOf<Project>(isList:false, isAsync:true)),
                new StoredProcedureDefintion("dbo", "ProjectUpsert",
                    CSTypeDefinition.TypeOf<Project>(),
                    ExecutionMode.QueryMultiple,
                    TypeOfTManipulationResult<ProjectManipulationResult, Project>()),

                new StoredProcedureDefintion("dbo", "ToDoDeletePK",
                    CSTypeDefinition.TypeOf<ToDo>(), 
                    ExecutionMode.Query,
                    CSTypeDefinition.TypeOf<ToDoPK>(isList:true, isAsync:true)),
                new StoredProcedureDefintion("dbo", "ToDoSelectPK",
                    CSTypeDefinition.TypeOf<ToDoPK>(), 
                    ExecutionMode.QuerySingleOrDefault,
                    CSTypeDefinition.TypeOf<ToDo>(isList:false, isAsync:true)),
                new StoredProcedureDefintion("dbo", "ToDoUpsert",
                    CSTypeDefinition.TypeOf<ToDo>(),
                    ExecutionMode.QueryMultiple,
                    TypeOfTManipulationResult<ToDoManipulationResult, ToDo>()),

                new StoredProcedureDefintion("dbo", "UserDeletePK",
                    CSTypeDefinition.TypeOf<User>(),
                    ExecutionMode.Query,
                    CSTypeDefinition.TypeOf<UserPK>(isList:true, isAsync:true)),
                new StoredProcedureDefintion("dbo", "UserSelectPK",
                    CSTypeDefinition.TypeOf<UserPK>(),
                    ExecutionMode.QuerySingleOrDefault,
                    CSTypeDefinition.TypeOf<User>(isList:false, isAsync:true)),
                new StoredProcedureDefintion("dbo", "UserUpsert",
                    CSTypeDefinition.TypeOf<User>(),
                    ExecutionMode.QueryMultiple,
                    TypeOfTManipulationResult<UserManipulationResult, User>()),

                new StoredProcedureDefintion("dbo", "sp_alterdiagram", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_creatediagram", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_dropdiagram", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_helpdiagramdefinition", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_helpdiagrams", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_renamediagram", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
                new StoredProcedureDefintion("dbo", "sp_upgraddiagrams", CSTypeDefinition.None, ExecutionMode.Ignore, CSTypeDefinition.None),
             },
             IgnoreTypePropertyNames: new TypePropertyNames[] {
                new TypePropertyNames(
                    typeof(Brimborium.TestSample.Record.Activity),
                    new string[] {
                          nameof(Brimborium.TestSample.Record.Activity.Project)
                        , nameof(Brimborium.TestSample.Record.Activity.ProjectHistory)
                        , nameof(Brimborium.TestSample.Record.Activity.ToDo)
                        , nameof(Brimborium.TestSample.Record.Activity.ToDoHistory)
                        , nameof(Brimborium.TestSample.Record.Activity.User)
                        , nameof(Brimborium.TestSample.Record.Activity.UserHistory)
                }),
                new TypePropertyNames(
                    typeof(Brimborium.TestSample.Record.Project),
                    new string[] {
                          nameof(Brimborium.TestSample.Record.Project.Activity)
                        , nameof(Brimborium.TestSample.Record.Project.ToDo)
                }),
                new TypePropertyNames(
                    typeof(Brimborium.TestSample.Record.ToDo),
                    new string[] {
                          nameof(Brimborium.TestSample.Record.ToDo.Activity)
                        , nameof(Brimborium.TestSample.Record.ToDo.Project)
                        , nameof(Brimborium.TestSample.Record.ToDo.User)
                }),
                new TypePropertyNames(
                    typeof(Brimborium.TestSample.Record.User),
                    new string[] {
                          nameof(Brimborium.TestSample.Record.User.Activity)
                        , nameof(Brimborium.TestSample.Record.User.ToDo)
                }),
             }
        );
    }
}
