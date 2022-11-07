namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class fiveModel : PageModel
    {
        private readonly Logic _Logic;
        public string Way = "";
        public fiveModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "5");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
