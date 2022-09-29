using ChargerMessages;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Partition;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using LFA;

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

            //var remoteConfig = GrpcNetRemoteConfig
            //    .BindToLocalhost().WithProtoMessages(MessagesReflection.Descriptor);
           //var remoteConfig = GrpcNetRemoteConfig
           //    .BindToAllInterfaces(advertisedHost: configuration["ProtoActor:AdvertisedHost"])
           //    .WithProtoMessages(MessagesReflection.Descriptor);

            var remoteConfig = GrpcNetRemoteConfig
                .BindTo("localhost"/*"0.0.0.0", 8300*/)
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

    );


            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
    }
}