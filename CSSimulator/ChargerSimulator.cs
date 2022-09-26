using Proto;
using Proto.Cluster;

namespace CSSimulator;

public class ChargerSimulator : BackgroundService
{
    private readonly ActorSystem _actorSystem;

    public ChargerSimulator(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        var lightBulbs = new[] { "living_room_1", "living_room_2", "bedroom", "kitchen" };

        while (!stoppingToken.IsCancellationRequested)
        {
            var randomIdentity = lightBulbs[random.Next(lightBulbs.Length)];

            var ChargerGrainClient = _actorSystem
                .Cluster()
                .GetChargerGrain(randomIdentity);

            if (random.Next(2) > 0)
            {
                await ChargerGrainClient.StartCharging(stoppingToken);
            }
            else
            {
                await ChargerGrainClient.StopCharging(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
        }
    }
}