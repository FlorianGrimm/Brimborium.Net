namespace Brimborium.CodeGeneration.SQLRelated {
    public class UtilitySQLInfoTests {

        [Fact]
        public void ExtractDatabaseSchema_Test() {
            var connectionString = Helper.GetConnectionString();
            Assert.NotNull(connectionString);
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            var database = UtilitySQLInfo.GetDatabase(connection);
            Assert.NotNull(database);
            var act = UtilitySQLInfo.ExtractDatabaseSchema(database!, _=>true);
            Assert.Equal(act.Tables.Count, act.TableByName.Count);
            Assert.True(act.Tables.Count>0);
        }
    }
}
