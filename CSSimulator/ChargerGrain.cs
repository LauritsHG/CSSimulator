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
    private PID? currentChargerGateway; //Current actor handling connection
    private string identity=""; //Serial number

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
            if(currentChargerGateway is not null) Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload = "Turn on, please :)",CommandUid= Guid.NewGuid().ToString() }) ;

            _state = ChargerState.Charging;
            await Task.Delay(0);
        }
    }

    public override async Task StopCharging()
    {
        if (_state != ChargerState.Charging)
        {
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger off");
            if(currentChargerGateway is not null) Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload = "Turn off, please :)" });

            _state = ChargerState.Idle;
            await Task.Delay(0);
        }
    }

    public override async Task  ReceiveMsgFromCharger(MessageFromCharger request)
    {

        Console.WriteLine(request.Msg + " from " + request.From);
        CommandToChargerMessage cmd = new()
        {
            Payload = request.Msg
        };
        if (currentChargerGateway is not null) Context.Send(currentChargerGateway, cmd);
        //await StartCharging();//Democode - not final
        await Task.Delay(0);
    }

    public override async Task NewWebSocketFromCharger(ChargerActorIdentity request)
    {
        currentChargerGateway = new PID
        {
            Address = request.Pid.Address,
            Id = request.Pid.Id
        };
        //currentChargerGateway =request.Pid;
        identity =request.SerialNumber;
        await Task.Delay(0);
    }

    public override async Task CommandReceived(CommandStatus status)
    {
        Console.WriteLine("Command received by charger "+identity+". Succeeded=" + status.Succeeded + " - " + status.Details);
        await Task.Delay(0);
    }
}