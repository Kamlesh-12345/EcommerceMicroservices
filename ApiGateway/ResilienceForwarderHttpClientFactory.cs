using System.Net.Http;
using Yarp.ReverseProxy.Forwarder;

public sealed class ResilienceForwarderHttpClientFactory : IForwarderHttpClientFactory
{
    private readonly IHttpMessageHandlerFactory _handlerFactory;

    public ResilienceForwarderHttpClientFactory(IHttpMessageHandlerFactory handlerFactory)
        => _handlerFactory = handlerFactory;

    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        var handler = _handlerFactory.CreateHandler("yarp-resilient");
        return new HttpMessageInvoker(handler, disposeHandler: false);
    }
}