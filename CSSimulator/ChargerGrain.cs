using Proto;
using Proto.Cluster;
using ChargerMessages;
using LFA;

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

        Console.WriteLine($"{_clusterIdentity.Identity}: new virtual grain actor created");
    }

    public override async Task StartCharging()
    {
        if (_state != ChargerState.Idle)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger on");
            Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload="Turn on, please :)"});

            _state = ChargerState.Charging;
        }
    }

    public override async Task StopCharging()
    {
        if (_state != ChargerState.Charging)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger off");
            Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload = "Turn off, please :)" });

            _state = ChargerState.Idle;
        }
    }

    public override async Task  ReceiveMsgFromCharger(MessageFromCharger request)
    {
        Console.WriteLine(request.Msg + " from " + request.From);
        CommandToChargerMessage cmd = new CommandToChargerMessage();
        cmd.Payload = request.Msg;
        Context.Send(currentChargerGateway, cmd);
        //await StartCharging();//Democode - not final
    }

    public override async Task NewWebSocketFromCharger(ChargerActorIdentity request)
    {
        currentChargerGateway=new PID();
        currentChargerGateway.Address = request.Pid.Address;
        currentChargerGateway.Id = request.Pid.Id;
        //currentChargerGateway =request.Pid;
        identity=request.SerialNumber;
    }
}