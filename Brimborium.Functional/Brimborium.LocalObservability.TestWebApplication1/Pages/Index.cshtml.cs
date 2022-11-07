namespace Brimborium.LocalObservability.TestWebApplication1.Pages {
    public class IndexModel : PageModel {
        private readonly Logic _Logic;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(Logic logic, ILogger<IndexModel> logger) {
            this._Logic = logic;
            this._logger = logger;
        }

        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "home");
            return this._Logic.StepEffect(step, this);
        }
    }
}