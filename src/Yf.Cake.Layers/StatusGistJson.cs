namespace Yf.Cake.Layers
{
    public sealed class StatusGistJson
    {
        public int SchemaVersion { get; } = 1;
        public required string Label { get; set; }
        public required string Message { get; set; }
        public required string Color { get; set; }
    }
}
