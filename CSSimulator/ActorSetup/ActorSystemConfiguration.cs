using ChargerMessages;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Cluster.Partition;
using Proto.DependencyInjection;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using LFA;
using Proto.Cluster.Kubernetes;

namespace CSSimulator.ActorSetup;

public static class ActorSystemConfiguration
{
    public static void AddActorSystem(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton(provider =>
        {
            // actor system configuration

            var actorSystemConfig = ActorSystemConfig
                .Setup().WithActorRequestTimeout(TimeSpan.FromMinutes(60));

            // remote configuration

            //var remoteConfig = GrpcNetRemoteConfig
            //    .BindToLocalhost().WithProtoMessages(MessagesReflection.Descriptor);
            //var remoteConfig = GrpcNetRemoteConfig
            //    .BindToAllInterfaces(advertisedHost: configuration["ProtoActor:AdvertisedHost"])
            //    .WithProtoMessages(MessagesReflection.Descriptor);

            var remoteConfig = GrpcNetRemoteConfig
                .BindToAllInterfaces(advertisedHost: configuration["ProtoActor:AdvertisedHost"])
                .WithProtoMessages(new[] { MessagesReflection.Descriptor, ChargerGatewayMessagesReflection.Descriptor });

            // cluster configuration

            var clusterConfig = ClusterConfig
                .Setup(
                    clusterName: "CSSimulatorCluster",
                    clusterProvider: new KubernetesProvider(),
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

    ).WithGossipRequestTimeout(TimeSpan.FromMinutes(60)).WithTimeout(TimeSpan.FromMinutes(60)).WithActorSpawnTimeout(TimeSpan.FromMinutes(60)).WithActorRequestTimeout(TimeSpan.FromMinutes(60)).WithActorActivationTimeout(TimeSpan.FromMinutes(60));


            // create the actor system

            return new ActorSystem(actorSystemConfig)
                .WithServiceProvider(provider)
                .WithRemote(remoteConfig)
                .WithCluster(clusterConfig);
        });
    }
}