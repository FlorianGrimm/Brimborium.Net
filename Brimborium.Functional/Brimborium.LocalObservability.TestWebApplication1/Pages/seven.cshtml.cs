using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class sevenModel : PageModel
    {
        private readonly Logic _Logic;
        public string Way = "";
        public sevenModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "7");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
