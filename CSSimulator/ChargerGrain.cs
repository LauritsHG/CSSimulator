using Proto;
using Proto.Cluster;
//using Messages;

namespace CSSimulator;

public class ChargerGrain : ChargerGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;

    private enum ChargerState { Unknown, Charging, Idle }
    private ChargerState _state = ChargerState.Unknown;

    public ChargerGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;

        Console.WriteLine($"{_clusterIdentity.Identity}: created");
    }

    public override async Task StartCharging()
    {
        if (_state != ChargerState.Idle)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger on");

            _state = ChargerState.Charging;
        }
    }

    public override async Task StopCharging()
    {
        if (_state != ChargerState.Charging)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger off");

            _state = ChargerState.Idle;
        }
    }

    public override Task ReceiveMsgFromCharger(MessageFromCharger request)
    {
        
        return Task.CompletedTask;
    }





    //public override async Task ReceiveMsgFromCharger(string msg, string from)
    //{

    //}

}