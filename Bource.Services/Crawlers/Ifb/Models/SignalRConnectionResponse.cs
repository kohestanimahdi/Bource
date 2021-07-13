namespace Bource.Services.Crawlers.Ifb.Models
{
    internal class SignalRConnectionResponse
    {
        public string Url { get; set; }
        public string ConnectionToken { get; set; }
        public string ConnectionId { get; set; }
        public float KeepAliveTimeout { get; set; }
        public float DisconnectTimeout { get; set; }
        public bool TryWebSockets { get; set; }
        public string ProtocolVersion { get; set; }
    }
}
