namespace Brimborium.Configuration.Test;

public class SqlConfigurationTest {
    // [Fact]
    public async Task SqlConfiguration001Test() {
        var args = new string[] { };
        var sqlConfigurationAccess = new TestSqlConfigurationAccess();
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configuration) => {
                configuration.Sources.Clear();
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string>() {
                        { "Database", "6*7" }
                    });

                configuration.AddSqlConfiguration(
                    connectionStringConfigurationName: "Database",
                    connectionString: "",
                    sqlConfigurationAccess
                    );
            })
            .ConfigureServices((context, services) => {
                services.AddOptions<HugoOptions>()
                    .Configure((hugoOptions) => {
                        context.Configuration.Bind("Hugo", hugoOptions);
                    });
                services.AddSingleton<HugoService>();
            })
            .Build();
        var optionsMonitor1 = host.Services.GetRequiredService<IOptionsMonitor<HugoOptions>>();
        var optionsValue = optionsMonitor1.CurrentValue;
        Assert.NotNull(optionsMonitor1.CurrentValue);
        Assert.Equal("42", optionsValue.Gna);
        optionsMonitor1.OnChange((o, n) => {
            optionsValue = o;
        });
        sqlConfigurationAccess.Set(
            new Dictionary<string, string>() {
                { "Hugo:Gna", "21" }
            });
        await Task.Delay(1000);
        var options2 = host.Services.GetRequiredService<IOptionsMonitor<HugoOptions>>().CurrentValue;
        Assert.NotNull(options2);
        Assert.Equal("21", optionsValue.Gna);
        Assert.Equal("42", options2.Gna);
    }

    [Fact]
    public async Task SqlConfiguration002Test() {
        var args = new string[] { };
        var sqlConfigurationAccess = new TestSqlConfigurationAccess();
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configuration) => {
                configuration.Sources.Clear();
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string>() {
                        { "Database", "6*7" }
                    });

                configuration.AddSqlConfiguration(
                    connectionStringConfigurationName: "Database",
                    connectionString: "",
                    sqlConfigurationAccess
                    );
            })
            .ConfigureServices((context, services) => {
                services.AddOptions<HugoOptions>()
                    .Configure((hugoOptions) => {
                        context.Configuration.Bind("Hugo", hugoOptions);
                    });
                services.AddSingleton<HugoService>();
            })
            .Build();
        var hugoService = host.Services.GetRequiredService<HugoService>();
        Assert.NotNull(hugoService);
        Assert.NotNull(hugoService.Options);
        Assert.Equal("42", hugoService.Options.Gna);
        sqlConfigurationAccess.Set(
                new Dictionary<string, string>() {
                { "Hugo:Gna", "21" }
                });
        await Task.Delay(1000);
        Assert.Equal("42", hugoService.Options.Gna);

        //await host.RunAsync();
    }

    class TestSqlConfigurationAccess : SqlConfigurationAccess {
        public TestSqlConfigurationAccess() {
            this.Data = new Dictionary<string, string>();
            this.Data.Add("Hugo:Gna", "42");
        }

        public Dictionary<string, string> Data { get; }

        public override bool Load(string connectionString, Dictionary<string, string> data) {
            Assert.Equal("6*7", connectionString);
            foreach (var (k, v) in this.Data) {
                data[k] = v;
            }
            return true;
        }
        public void Set(Dictionary<string, string> data) {
            this.Data.Clear();
            foreach (var (k, v) in data) {
                this.Data[k] = v;
            }
            base.OnReload();
        }
    }
}