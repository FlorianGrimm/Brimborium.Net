namespace Brimborium.Typing {
    public interface IMetaName {
        string ContainerName { get; set; }
        string FullName { get; }
        string Name { get; set; }
        string Namespace { get; set; }
    }
}