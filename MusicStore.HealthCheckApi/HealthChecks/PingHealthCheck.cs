using System.Net.NetworkInformation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MusicStore.HealthCheckApi.HealthChecks;

public class PingHealthCheck : IHealthCheck
{
    private readonly string _host;

    public PingHealthCheck(string host)
    {
        _host = host;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var ping = new Ping();

        HealthCheckResult result;

        try
        {
            var reply = await ping.SendPingAsync(_host, 2000);

            switch (reply.Status)
            {
                case IPStatus.Success:
                    result = HealthCheckResult.Healthy($"Todo un exito el host {_host}");
                    break;

                case IPStatus.BadDestination:
                    result = HealthCheckResult.Unhealthy($"El host {_host} no existe");
                    break;

                case IPStatus.TimedOut:
                case IPStatus.TimeExceeded:
                    result = HealthCheckResult.Degraded($"El Host {_host} esta lento");
                    break;

                default:
                    result = HealthCheckResult.Unhealthy($"El host {_host} no se puede hacer ping");
                    break;
            }
        }
        catch (Exception ex)
        {
            result = HealthCheckResult.Unhealthy(ex.Message);
        }

        return result;
    }
}