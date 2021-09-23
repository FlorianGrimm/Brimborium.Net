using Demo.Model;

using System.Collections.Generic;

namespace Demo.Communication {
    public record EbbesQueryRequest(
        string Pattern
    );

    public record EbbesQueryResponse(
        List<EbbesModel> Items
    );
}
