namespace Brimborium.LocalObservability.TestWebApplication1.Pages {
    public class threeModel : PageModel {
        private readonly Logic _Logic;
        private readonly ILogger<threeModel> _Logger;
        public string Way = "";
        public threeModel(
            Logic logic,
            ILogger<threeModel> logger
            ) {
            this._Logic = logic;
            this._Logger = logger;
        }

        public IActionResult OnGet() {
            this._Logger.LogCodepoint(LogLevel.Information, OnGetThree, "TopSecret", "Should not be Logged");
            this._Logger.LogInformation("OnGetThree");
            var step = this._Logic.Step(this.HttpContext.Session, "3");
            this.Way = step.Way;
            return this._Logic.StepEffect(step, this);
        }

        private static CodePoint OnGetThree = new CodePoint(nameof(threeModel), nameof(OnGet), 1);
    }
}
