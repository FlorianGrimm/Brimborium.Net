namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class sixModel : PageModel
    {
        private readonly Logic _Logic;
        public string Way = "";
        public sixModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "6");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
