using Brimborium.WebFlow.Web.Model;

using System.Collections.Generic;

namespace Brimborium.WebFlow.Web.Communication {
    public record GnaQueryRequest(
        string Pattern
    );

    public record GnaQueryResponse(
        List<GnaModel> Items
    );
}
