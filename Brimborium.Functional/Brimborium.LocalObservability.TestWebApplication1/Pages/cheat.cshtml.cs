using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class cheatModel : PageModel
    {
        private readonly Logic _Logic;

        public cheatModel(Logic logic) {
            this._Logic = logic;
        }

        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "cheat");
            return this._Logic.StepEffect(step, this);
        }
    }
}
