namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class fourModel : PageModel
    {
        private readonly Logic _Logic;
        public string Way = "";
        public fourModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "4");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
