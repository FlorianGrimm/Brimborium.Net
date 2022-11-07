namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class okModel : PageModel
    {
        private readonly Logic _Logic;

        public okModel(Logic logic) {
            this._Logic = logic;
        }

        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "ok");
            return this._Logic.StepEffect(step, this);
        }
    }
}
