namespace Demo.Communication {
    public record GnaUpsertRequest(
        string Name,
        bool Done
    );

    public record GnaUpsertResponse(
        // string Error
    );
}
