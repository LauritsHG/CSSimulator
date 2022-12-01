using Proto;
using Proto.Cluster;
using ChargerMessages;
using LFA;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

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

        Debug.WriteLine($"{_clusterIdentity.Identity}: new virtual grain actor created");
    }

    public override async Task StartCharging()
    {
        if (_state != ChargerState.Charging)
        {
            string newCommandUid = Guid.NewGuid().ToString();
            addCommand(newCommandUid, "Started");
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger on. Timestamp: " + DateTime.Now.ToString("T") + ":" + DateTime.Now.ToString("ff"));
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
            Console.WriteLine($"{_clusterIdentity.Identity}: turning charger off. Timestamp: " + DateTime.Now.ToString("T")+":"+DateTime.Now.ToString("ff"));
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

        if (currentChargerGateway == null)
        {
            var pid = new PID
            {
                Address = request.Pid.Address,
                Id = request.Pid.Id
            };
            Context.Send(pid, new ResendSetup());
        }

        string message = request.Msg.Split("\0")[0]; // Removes empty characters
        Console.WriteLine("Received msg " + message+ "  Timestamp: " + DateTime.Now.ToString("T") + ":" + DateTime.Now.ToString("ff"));
        ChargerGrainStorage.UpdateLastMessage(message, index); 
    }

    public override async Task NewWebSocketFromCharger(ChargerActorIdentity request)
    {
        currentChargerGateway = new PID
        {
            Address = request.Pid.Address,
            Id = request.Pid.Id
        };
        ChargerGrainStorage.chargerGrains.ElementAt(index).identity = request.SerialNumber;
        ChargerGrainStorage.UpdateLastMessage("New Connection".Split("\0")[0], index); // Removes empty characters
    }

    public override async Task CommandReceived(CommandStatus status)
    {
        Console.WriteLine("Command received by charger "+identity+". Succeeded=" + status.Succeeded + " - " + status.Details+ ".  Timestamp: " + DateTime.Now.ToString("T") + ":" + DateTime.Now.ToString("ff"));

        if (sentCommands[status.CommandUid.Split("\0")[0]]!= null)
        {
            ChargerGrainStorage.UpdateStatus(sentCommands[status.CommandUid.Split("\0")[0]], index);
            sentCommands.Remove(status.CommandUid.Split("\0")[0]);
        }
    }
}