using System.Net.Http;
using Yarp.ReverseProxy.Forwarder;

public sealed class ResilienceForwarderHttpClientFactory : IForwarderHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ResilienceForwarderHttpClientFactory(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
        => _httpClientFactory.CreateClient("yarp-resilient");
}