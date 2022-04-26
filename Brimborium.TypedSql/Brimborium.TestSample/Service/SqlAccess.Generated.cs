#if true
#nullable enable
namespace Brimborium.TestSample.Service {
    public partial class SqlAccess {
        public async Task<Brimborium.TestSample.Record.Activity> ExecuteActivityInsertAsync(Brimborium.TestSample.Record.Activity args)  {
            using(var cmd = this.CreateCommand("[dbo].[ActivityInsert]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterString(cmd, "@Title", SqlDbType.NVarChar, 20, args.Title);
                this.AddParameterString(cmd, "@Data", SqlDbType.NVarChar, -1, args.Data);
                this.AddParameterDateTimeOffset(cmd, "@CreatedAt", args.CreatedAt);
                return await this.CommandQuerySingleAsync<Brimborium.TestSample.Record.Activity>(cmd, ReadRecordActivityInsert);
            }
        } 

        protected Brimborium.TestSample.Record.Activity ReadRecordActivityInsert(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.Activity() {
                Id = this.ReadGuid(reader, 0),
                Title = this.ReadString(reader, 1),
                EntityType = this.ReadString(reader, 2),
                EntityId = this.ReadString(reader, 3),
                Data = this.ReadString(reader, 4),
                CreatedAt = this.ReadDateTimeOffset(reader, 5),
                SerialVersion = this.ReadInt64(reader, 6)
            } ;
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.Activity?> ExecuteActivitySelectPKAsync(Brimborium.TestSample.Record.ActivityPK args)  {
            using(var cmd = this.CreateCommand("[dbo].[ActivitySelectPK]", CommandType.StoredProcedure)) {
                this.AddParameterDateTimeOffset(cmd, "@CreatedAt", args.CreatedAt);
                this.AddParameterGuid(cmd, "@Id", args.Id);
                return await this.CommandQuerySingleOrDefaultAsync<Brimborium.TestSample.Record.Activity>(cmd, ReadRecordActivitySelectPK);
            }
        } 

        protected Brimborium.TestSample.Record.Activity ReadRecordActivitySelectPK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.Activity() {
                Id = this.ReadGuid(reader, 0),
                Title = this.ReadString(reader, 1),
                EntityType = this.ReadString(reader, 2),
                EntityId = this.ReadString(reader, 3),
                Data = this.ReadString(reader, 4),
                CreatedAt = this.ReadDateTimeOffset(reader, 5),
                SerialVersion = this.ReadInt64(reader, 6)
            } ;
            return result;
        } 

        public async Task<List<Brimborium.TestSample.Record.ProjectPK>> ExecuteProjectDeletePKAsync(Brimborium.TestSample.Record.Project args)  {
            using(var cmd = this.CreateCommand("[dbo].[ProjectDeletePK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                return await this.CommandQueryAsync<Brimborium.TestSample.Record.ProjectPK>(cmd, ReadRecordProjectDeletePK);
            }
        } 

        protected Brimborium.TestSample.Record.ProjectPK ReadRecordProjectDeletePK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.ProjectPK(
                @Id: this.ReadGuid(reader, 0)
            );
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.Project?> ExecuteProjectSelectPKAsync(Brimborium.TestSample.Record.ProjectPK args)  {
            using(var cmd = this.CreateCommand("[dbo].[ProjectSelectPK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                return await this.CommandQuerySingleOrDefaultAsync<Brimborium.TestSample.Record.Project>(cmd, ReadRecordProjectSelectPK);
            }
        } 

        protected Brimborium.TestSample.Record.Project ReadRecordProjectSelectPK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.Project() {
                Id = this.ReadGuid(reader, 0),
                Title = this.ReadString(reader, 1),
                ActivityId = this.ReadGuidQ(reader, 2),
                CreatedAt = this.ReadDateTimeOffset(reader, 3),
                ModifiedAt = this.ReadDateTimeOffset(reader, 4),
                SerialVersion = this.ReadInt64(reader, 5)
            } ;
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.ProjectManipulationResult> ExecuteProjectUpsertAsync(Brimborium.TestSample.Record.Project args)  {
            using(var cmd = this.CreateCommand("[dbo].[ProjectUpsert]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterString(cmd, "@Title", SqlDbType.NVarChar, 50, args.Title);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@CreatedAt", args.CreatedAt);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                Brimborium.TestSample.Record.Project result_DataResult = default!;
                Brimborium.TestSample.Record.OperationResult result_OperationResult = default!;
                await this.CommandQueryMultipleAsync(cmd, async (idx, reader) => {
                    if (idx == 0) {
                        result_DataResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.Project>(reader, ReadRecordProjectUpsert_0);
                    }
                    if (idx == 1) {
                        result_OperationResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.OperationResult>(reader, ReadRecordProjectUpsert_1);
                    }
                } , 2);
                var result = new Brimborium.TestSample.Record.ProjectManipulationResult(
                    DataResult: result_DataResult,
                    OperationResult: result_OperationResult
                );
                return result;
            }
        } 

        protected Brimborium.TestSample.Record.Project ReadRecordProjectUpsert_0(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.Project() {
                Id = this.ReadGuid(reader, 0),
                Title = this.ReadString(reader, 1),
                ActivityId = this.ReadGuidQ(reader, 2),
                CreatedAt = this.ReadDateTimeOffset(reader, 3),
                ModifiedAt = this.ReadDateTimeOffset(reader, 4),
                SerialVersion = this.ReadInt64(reader, 5)
            } ;
            return result;
        } 

        protected Brimborium.TestSample.Record.OperationResult ReadRecordProjectUpsert_1(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.OperationResult(
                @resultValue: (Brimborium.TestSample.Record.ResultValue) (this.ReadInt32(reader, 0))
            ) {
                Message = this.ReadString(reader, 1)
            } ;
            return result;
        } 

        public async Task<List<Brimborium.TestSample.Record.ToDoPK>> ExecuteToDoDeletePKAsync(Brimborium.TestSample.Record.ToDo args)  {
            using(var cmd = this.CreateCommand("[dbo].[ToDoDeletePK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                return await this.CommandQueryAsync<Brimborium.TestSample.Record.ToDoPK>(cmd, ReadRecordToDoDeletePK);
            }
        } 

        protected Brimborium.TestSample.Record.ToDoPK ReadRecordToDoDeletePK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.ToDoPK(
                @Id: this.ReadGuid(reader, 0)
            );
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.ToDo?> ExecuteToDoSelectPKAsync(Brimborium.TestSample.Record.ToDoPK args)  {
            using(var cmd = this.CreateCommand("[dbo].[ToDoSelectPK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                return await this.CommandQuerySingleOrDefaultAsync<Brimborium.TestSample.Record.ToDo>(cmd, ReadRecordToDoSelectPK);
            }
        } 

        protected Brimborium.TestSample.Record.ToDo ReadRecordToDoSelectPK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.ToDo() {
                Id = this.ReadGuid(reader, 0),
                ProjectId = this.ReadGuidQ(reader, 1),
                UserId = this.ReadGuidQ(reader, 2),
                Title = this.ReadString(reader, 3),
                Done = this.ReadBoolean(reader, 4),
                ActivityId = this.ReadGuidQ(reader, 5),
                CreatedAt = this.ReadDateTimeOffset(reader, 6),
                ModifiedAt = this.ReadDateTimeOffset(reader, 7),
                SerialVersion = this.ReadInt64(reader, 8)
            } ;
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.ToDoManipulationResult> ExecuteToDoUpsertAsync(Brimborium.TestSample.Record.ToDo args)  {
            using(var cmd = this.CreateCommand("[dbo].[ToDoUpsert]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterGuid(cmd, "@ProjectId", args.ProjectId);
                this.AddParameterGuid(cmd, "@UserId", args.UserId);
                this.AddParameterString(cmd, "@Title", SqlDbType.NVarChar, 50, args.Title);
                this.AddParameterBoolean(cmd, "@Done", args.Done);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@CreatedAt", args.CreatedAt);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                Brimborium.TestSample.Record.ToDo result_DataResult = default!;
                Brimborium.TestSample.Record.OperationResult result_OperationResult = default!;
                await this.CommandQueryMultipleAsync(cmd, async (idx, reader) => {
                    if (idx == 0) {
                        result_DataResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.ToDo>(reader, ReadRecordToDoUpsert_0);
                    }
                    if (idx == 1) {
                        result_OperationResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.OperationResult>(reader, ReadRecordToDoUpsert_1);
                    }
                } , 2);
                var result = new Brimborium.TestSample.Record.ToDoManipulationResult(
                    DataResult: result_DataResult,
                    OperationResult: result_OperationResult
                );
                return result;
            }
        } 

        protected Brimborium.TestSample.Record.ToDo ReadRecordToDoUpsert_0(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.ToDo() {
                Id = this.ReadGuid(reader, 0),
                ProjectId = this.ReadGuidQ(reader, 1),
                UserId = this.ReadGuidQ(reader, 2),
                Title = this.ReadString(reader, 3),
                Done = this.ReadBoolean(reader, 4),
                ActivityId = this.ReadGuidQ(reader, 5),
                CreatedAt = this.ReadDateTimeOffset(reader, 6),
                ModifiedAt = this.ReadDateTimeOffset(reader, 7),
                SerialVersion = this.ReadInt64(reader, 8)
            } ;
            return result;
        } 

        protected Brimborium.TestSample.Record.OperationResult ReadRecordToDoUpsert_1(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.OperationResult(
                @resultValue: (Brimborium.TestSample.Record.ResultValue) (this.ReadInt32(reader, 0))
            ) {
                Message = this.ReadString(reader, 1)
            } ;
            return result;
        } 

        public async Task<List<Brimborium.TestSample.Record.UserPK>> ExecuteUserDeletePKAsync(Brimborium.TestSample.Record.User args)  {
            using(var cmd = this.CreateCommand("[dbo].[UserDeletePK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                return await this.CommandQueryAsync<Brimborium.TestSample.Record.UserPK>(cmd, ReadRecordUserDeletePK);
            }
        } 

        protected Brimborium.TestSample.Record.UserPK ReadRecordUserDeletePK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.UserPK(
                @Id: this.ReadGuid(reader, 0)
            );
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.User?> ExecuteUserSelectPKAsync(Brimborium.TestSample.Record.UserPK args)  {
            using(var cmd = this.CreateCommand("[dbo].[UserSelectPK]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                return await this.CommandQuerySingleOrDefaultAsync<Brimborium.TestSample.Record.User>(cmd, ReadRecordUserSelectPK);
            }
        } 

        protected Brimborium.TestSample.Record.User ReadRecordUserSelectPK(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.User() {
                Id = this.ReadGuid(reader, 0),
                UserName = this.ReadString(reader, 1),
                ActivityId = this.ReadGuidQ(reader, 2),
                CreatedAt = this.ReadDateTimeOffset(reader, 3),
                ModifiedAt = this.ReadDateTimeOffset(reader, 4),
                SerialVersion = this.ReadInt64(reader, 5)
            } ;
            return result;
        } 

        public async Task<Brimborium.TestSample.Record.UserManipulationResult> ExecuteUserUpsertAsync(Brimborium.TestSample.Record.User args)  {
            using(var cmd = this.CreateCommand("[dbo].[UserUpsert]", CommandType.StoredProcedure)) {
                this.AddParameterGuid(cmd, "@Id", args.Id);
                this.AddParameterString(cmd, "@UserName", SqlDbType.NVarChar, 50, args.UserName);
                this.AddParameterGuid(cmd, "@ActivityId", args.ActivityId);
                this.AddParameterDateTimeOffset(cmd, "@CreatedAt", args.CreatedAt);
                this.AddParameterDateTimeOffset(cmd, "@ModifiedAt", args.ModifiedAt);
                this.AddParameterLong(cmd, "@SerialVersion", args.SerialVersion);
                Brimborium.TestSample.Record.User result_DataResult = default!;
                Brimborium.TestSample.Record.OperationResult result_OperationResult = default!;
                await this.CommandQueryMultipleAsync(cmd, async (idx, reader) => {
                    if (idx == 0) {
                        result_DataResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.User>(reader, ReadRecordUserUpsert_0);
                    }
                    if (idx == 1) {
                        result_OperationResult = await this.CommandReadQuerySingleAsync<Brimborium.TestSample.Record.OperationResult>(reader, ReadRecordUserUpsert_1);
                    }
                } , 2);
                var result = new Brimborium.TestSample.Record.UserManipulationResult(
                    DataResult: result_DataResult,
                    OperationResult: result_OperationResult
                );
                return result;
            }
        } 

        protected Brimborium.TestSample.Record.User ReadRecordUserUpsert_0(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.User() {
                Id = this.ReadGuid(reader, 0),
                UserName = this.ReadString(reader, 1),
                ActivityId = this.ReadGuidQ(reader, 2),
                CreatedAt = this.ReadDateTimeOffset(reader, 3),
                ModifiedAt = this.ReadDateTimeOffset(reader, 4),
                SerialVersion = this.ReadInt64(reader, 5)
            } ;
            return result;
        } 

        protected Brimborium.TestSample.Record.OperationResult ReadRecordUserUpsert_1(Microsoft.Data.SqlClient.SqlDataReader reader) {
            var result = new Brimborium.TestSample.Record.OperationResult(
                @resultValue: (Brimborium.TestSample.Record.ResultValue) (this.ReadInt32(reader, 0))
            ) {
                Message = this.ReadString(reader, 1)
            } ;
            return result;
        } 

    }
}

#endif
