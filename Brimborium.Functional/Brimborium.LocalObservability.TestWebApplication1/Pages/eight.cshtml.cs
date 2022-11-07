namespace Brimborium.LocalObservability.TestWebApplication1.Pages {
    public class eightModel : PageModel {
        private readonly Logic _Logic;
        public string Way = "";
        public eightModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "8");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
