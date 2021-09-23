namespace Demo.Communication {
    public record EbbesUpsertRequest(
        string Name,
        bool Done
    );

    public record EbbesUpsertResponse(
    );
}
