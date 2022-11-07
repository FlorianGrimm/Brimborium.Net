namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class failModel : PageModel
    {
        private readonly Logic _Logic;

        public failModel(Logic logic) {
            this._Logic = logic;
        }

        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "fail");
            return this._Logic.StepEffect(step, this);
        }
    }
}
