using Proto;
using Proto.Cluster;
//using Messages;

namespace CSSimulator;

public class ChargerGrain : ChargerGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;

    private enum ChargerState { Unknown, Charging, Idle }
    private ChargerState _state = ChargerState.Unknown;
    private PID currentChargerGateway; //Current actor handling connection
    private string identity; //Serial number

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

    public override async Task  ReceiveMsgFromCharger(MessageFromCharger request)
    {
        Console.WriteLine(request.Msg + " from " + request.From);
        await Task.CompletedTask;
    }

    public override async Task NewWebSocketFromCharger(ChargerActorIdentity request)
    {
        currentChargerGateway = request.Pid;
        identity=request.SerialNumber;
        Console.WriteLine("New connection from Charger registered: "+ identity);
    }
}