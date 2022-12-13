using ChargerMessages;
using LFA;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Partition;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;


namespace CSSimulator.ActorSetup;
/// <summary>
/// Configure actorsystem for communication between LFA and CS
/// Prerequisites: Download and run Consul: https://www.consul.io/downloads
/// </summary>
public static class ActorSystemConfigurationConsul
{
    public static void AddConsulActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup().WithActorRequestTimeout(TimeSpan.FromMinutes(60));
            //var actorSystemConfig = new ActorSystemConfig
            //{
            //    ActorRequestTimeout = TimeSpan.FromMinutes(5),
            //    DeadLetterRequestLogging = false
            //};

            // remote configuration

            var remoteConfig = GrpcNetRemoteConfig
                .BindToLocalhost()
                .WithProtoMessages(new[] { MessagesReflection.Descriptor, ChargerGatewayMessagesReflection.Descriptor });

            // cluster configuration

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "CSSimulatorCluster",
                    clusterProvider: new ConsulProvider(new ConsulProviderConfig()),
                    identityLookup: new PartitionIdentityLookup()
                ).WithClusterKind(
                kind: ChargerGrainActor.Kind,
                prop: Props.FromProducer(() =>
            new ChargerGrainActor(
                (context, clusterIdentity) => new ChargerGrain(context, clusterIdentity)
            )
        )

    ).WithClusterKind(
                kind: AuthGrainActor.Kind,
                prop: Props.FromProducer(() =>
            new AuthGrainActor(
                (context, clusterIdentity) => new AuthGrain(context, clusterIdentity)
            )
        )

    ).WithGossipRequestTimeout(TimeSpan.FromMinutes(60))
    .WithTimeout(TimeSpan.FromMinutes(60))
    .WithActorSpawnTimeout(TimeSpan.FromMinutes(60))
    .WithActorRequestTimeout(TimeSpan.FromMinutes(60))
    .WithActorActivationTimeout(TimeSpan.FromMinutes(60));

            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
    }
}
