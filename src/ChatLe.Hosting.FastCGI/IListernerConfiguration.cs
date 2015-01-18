namespace ChatLe.Hosting.FastCGI
{
    public interface IListernerConfiguration
    {
        int MaxConnections { get; set; }
        int MaxRequests { get; set; }
        bool SupportMultiplexing { get; set; }
    }
}