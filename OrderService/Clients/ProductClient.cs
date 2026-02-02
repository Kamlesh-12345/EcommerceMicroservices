using System.Net;
using System.Net.Http.Json;

namespace OrderService.Clients;

public sealed class ProductClient(HttpClient http)
{
    public async Task<(bool ok, HttpStatusCode status)> ReserveStockAsync(
        int productId,
        int quantity,
        CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(
            $"/products/{productId}/reserve",
            new { quantity },
            ct);

        return (resp.StatusCode == HttpStatusCode.OK, resp.StatusCode);
    }
}
