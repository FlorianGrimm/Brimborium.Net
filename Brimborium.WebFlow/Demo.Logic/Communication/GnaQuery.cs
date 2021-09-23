using Demo.Model;

using System.Collections.Generic;

namespace Demo.Communication {
    public record GnaQueryRequest(
        string Pattern
    );

    public record GnaQueryResponse(
        List<GnaModel> Items
    );
}
