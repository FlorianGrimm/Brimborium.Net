using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;

namespace Brimborium.LocalObservability.TestWebApplication1 {
    public partial class Logic {
        private readonly ILogger<Logic> _Logger;

        public Logic(
            ILogger<Logic> logger
            ) {
            this._Logger = logger;
        }

        public StepResult Step(ISession session, string page) {
            if (session is DistributedSession distributedSession) {
                this._Logger.LogInformation("distributedSession: {Id}", distributedSession.Id);
            }
            //this._Logger.LogInformation("type {0}",session.GetType().FullName);
            
            var way = session.GetString("way") ?? String.Empty;
            if (page == "home") {
                var value = "1-2";
                this._Logger.LogInformation("home {way}",value);
                session.SetString("way", value);
                return new StepResult(value, 0);
            }
            if (page == "ok") {
                if (way == "1-2-3-5-8") {
                    this._Logger.LogInformation("ok {way} good", way);
                    return new StepResult(way, 0);
                } else {
                    this._Logger.LogInformation("ok {way} cheat", way);
                    return new StepResult(way, 3);
                }
            }
            if (page == "fail") {
                this._Logger.LogInformation("fail {way}", way);
                return new StepResult(way, 0);
            }
            if (page == "cheat") {
                this._Logger.LogInformation("cheat {way}", way);
                return new StepResult(way, 0);
            }
            if (string.IsNullOrEmpty(way)) {
                this._Logger.LogInformation("empty");
                return new StepResult("", 4);
            }
            {
                var value = way + "-" + page;
                session.SetString("way", value);
                if (value == "1-2-3-5-8") {
                    this._Logger.LogInformation("found {way}", way);
                    return new StepResult(value, 1);
                }
                if (value.Length==9) {
                    this._Logger.LogInformation("failed {way}", way);
                    return new StepResult(value, 2);
                }
                this._Logger.LogInformation("step {way}", way);
                return new StepResult(value, 0);
            }
        }

        public ActionResult StepEffect(StepResult step, PageModel page) {
            if (step.Effect == 1) {
                return page.RedirectToPage("ok");
            }
            if (step.Effect == 2) {
                return page.RedirectToPage("fail");
            }
            if (step.Effect == 3) {
                return page.RedirectToPage("cheat");
            }
            if (step.Effect == 4) {
                return page.RedirectToPage("index   ");
            }
            return page.Page();
        }
    }
    public record StepResult(
        string Way,
        int Effect
        ) {
        public override string ToString() {
            return this.Way;
        }
    }
}
