using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Brimborium.LocalObservability.TestWebApplication1.Pages
{
    public class threeModel : PageModel
    {
        private readonly Logic _Logic;
        public string Way = "";
        public threeModel(Logic logic) {
            this._Logic = logic;
        }
        public IActionResult OnGet() {
            var step = this._Logic.Step(this.HttpContext.Session, "3");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }
    }
}
