using Proto;
using Proto.Cluster;
using ChargerMessages;
using LFA;
using System.Text;
using System.Text.RegularExpressions;

namespace CSSimulator;

public class ChargerGrain : ChargerGrainBase
{

    private readonly ClusterIdentity _clusterIdentity;

    private enum ChargerState { Unknown, Charging, Idle }
    private ChargerState _state = ChargerState.Unknown;
    private PID? currentChargerGateway; //Current actor handling connection
    private string identity; //Serial number
    public int index =-1; // should probably be in Storage instead;
    private Dictionary<String, String> sentCommands = new();

    public ChargerGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;

        if (index < 0) //only add new Grain if its a completely new charger.
        {
            ChargerGrainStorage.addChargerGrain(this, "unknown");
            index = ChargerGrainStorage.currentChargerGrainAmounts;
            ChargerGrainStorage.currentChargerGrainAmounts++;
        }

        Console.WriteLine($"{_clusterIdentity.Identity}: new virtual grain actor created");
    }

    public override async Task StartCharging()
    {
        if (_state != ChargerState.Charging)
        {
            string newCommandUid = Guid.NewGuid().ToString();
            addCommand(newCommandUid, "Started");
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger on");
            Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload = "Turn on, please :)", CommandUid= newCommandUid }) ;

            _state = ChargerState.Charging;
            await Task.Delay(0);
        }
    }

    public override async Task StopCharging()
    {
        if (_state != ChargerState.Idle)
        {
            string newCommandUid = Guid.NewGuid().ToString();
            addCommand(newCommandUid, "Stopped");
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger off");
            Context.Send(currentChargerGateway, new CommandToChargerMessage { Payload = "Turn off, please :)", CommandUid = newCommandUid });

            _state = ChargerState.Idle;
            await Task.Delay(0);
        }
    }
    private void addCommand(string uid, string commandMsg)
    {
        if (sentCommands.Count < 10)
        {
            sentCommands.Add(uid, commandMsg);
        }
        else
        {
            sentCommands.Remove(sentCommands.Keys.First());
            sentCommands.Add(uid, commandMsg);
        }
    }

    public override async Task  ReceiveMsgFromCharger(MessageFromCharger request)
    {

        //Console.WriteLine(request.Msg + " from " + request.From);
        ChargerGrainStorage.UpdateLastMessage(request.Msg.Split("\0")[0], index); // Removes empty characters

        //AuthenticationResponse response = new();
        //response.Validated = true;
        //if (currentChargerGateway == null) { response.Validated = false; }
        //return response;

        //CommandToChargerMessage cmd = new()
        //{
        //    Payload = request.Msg
        //};
        //Context.Send(currentChargerGateway, cmd); //
    }

    public override async Task NewWebSocketFromCharger(ChargerActorIdentity request)
    {
        currentChargerGateway = new PID
        {
            Address = request.Pid.Address,
            Id = request.Pid.Id
        };
        ChargerGrainStorage.chargerGrains.ElementAt(index)/*chargerGrains[index]*/.identity = request.SerialNumber;
        ChargerGrainStorage.UpdateLastMessage("New Connection".Split("\0")[0], index); // Removes empty characters
    }

    public override async Task CommandReceived(CommandStatus status)
    {
        Console.WriteLine("Command received by charger "+identity+". Succeeded=" + status.Succeeded + " - " + status.Details);
        //ChargerGrainStorage.UpdateLastMessage(status.Details.Split("\0")[0], index); // Removes empty characters
        if (sentCommands[status.CommandUid.Split("\0")[0]]!= null)
        {
            ChargerGrainStorage.UpdateStatus(sentCommands[status.CommandUid.Split("\0")[0]], index);
            sentCommands.Remove(status.CommandUid.Split("\0")[0]);
        }
    }
}