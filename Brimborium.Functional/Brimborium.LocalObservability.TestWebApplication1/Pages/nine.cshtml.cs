namespace Brimborium.LocalObservability.TestWebApplication1.Pages {
    public class nineModel : PageModel {
        private readonly Logic _Logic;
        public string Way = "";
        public nineModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "9");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
