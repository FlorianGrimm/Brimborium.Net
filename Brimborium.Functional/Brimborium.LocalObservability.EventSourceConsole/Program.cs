using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

class Program {
    static TraceEventSession? _Session;
    static void Main() {
        string providerName = "Brimborium-LocalObservability";
        System.Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            if (_Session != null) {
                _Session.Stop();
            }
            System.Console.WriteLine("bye.");
            e.Cancel = true;
        };
        doSesssion(providerName);
    }

    private static void doSesssion(string providerName) {
        // "MyEventData.etl"
        using (var session = new TraceEventSession("MySession")) {
            session.EnableProvider(providerName);
            _Session = session;
            var source = session.Source;

            Console.WriteLine(providerName);
            source.Dynamic.All += delegate (TraceEvent data) {
                Console.WriteLine(data.EventIndex);
                Console.WriteLine(data.ToString());
            };
            source.Process();
            // source.Dynamic.WriteAllManifests(".");
            _Session = null;
        }
    }

    //private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e) {
    //    throw new NotImplementedException();
    //}
}