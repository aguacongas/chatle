namespace ChatLe.Hosting.FastCGI
{
    public interface IConfiguration
    {
        int MaxConnections { get; set; }
        int MaxRequests { get; set; }
        bool SupportMultiplexing { get; set; }
    }
}