namespace Brimborium.GenerateStoredProcedure {
    public class Configuration {
        public Configuration() {
        }

        public virtual ConfigurationBound Build(DatabaseInfo databaseInfo) {
            var result = new ConfigurationBound();
            return result;
        }
    }
}
