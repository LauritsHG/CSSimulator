using Proto;
using Proto.Cluster;
using Proto.Cluster.Partition;
using Proto.Cluster.Testing;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;

namespace CSSimulator;

public static class ActorSystemConfiguration
{
    public static void AddActorSystem(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup();

            // remote configuration

            var remoteConfig = GrpcNetRemoteConfig
                .BindToLocalhost().WithProtoMessages();

            // cluster configuration

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "CSSimulatorCluster",
                    clusterProvider: new TestProvider(new TestProviderOptions(), new InMemAgent()),
                    identityLookup: new PartitionIdentityLookup()
                ).WithClusterKind(
                kind: ChargerGrainActor.Kind,
                prop: Props.FromProducer(() =>
            new ChargerGrainActor(
                (context, clusterIdentity) => new ChargerGrain(context, clusterIdentity)
            )
        )
    );

            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
    }
}