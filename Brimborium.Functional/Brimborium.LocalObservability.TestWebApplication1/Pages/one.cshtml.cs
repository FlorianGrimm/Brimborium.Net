namespace Brimborium.LocalObservability.TestWebApplication1.Pages {
    public class oneModel : PageModel {
        private readonly Logic _Logic;
        private readonly ILogger<oneModel> _Logger;
        public string Way = "";
        public oneModel(Logic logic, ILogger<oneModel> logger) {
            this._Logic = logic;
            this._Logger = logger;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "1");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
