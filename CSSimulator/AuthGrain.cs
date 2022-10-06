using Proto;
using Proto.Cluster;
using ChargerMessages;
using LFA;
using System.Text;

namespace CSSimulator;

public class AuthGrain : AuthGrainBase
{
    private readonly ClusterIdentity _clusterIdentity;


    public AuthGrain(IContext context, ClusterIdentity clusterIdentity) : base(context)
    {
        _clusterIdentity = clusterIdentity;

        Console.WriteLine($"{_clusterIdentity.Identity}: new virtual Auth grain actor created");
    }

    public override async Task<AuthenticationResponse> Authenticate(AuthenticationMessage request)
    {
        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Credentials));
        }
        catch(Exception exp)
        {
            decoded = "NOTVALID:NOTBASE64";
        }
        
        Console.WriteLine("Charger with Auth: " + decoded + " is connecting");
        string[] splitString = decoded.Split(":");
        AuthenticationResponse response = new AuthenticationResponse();
        if (splitString.Length != 2)
        {
            response.Validated = false;
        }
        else
        {
            if (splitString[0] == "Username" && splitString[1] == "Password")
            {
                response.Validated = true;
            }
            else
            {
                response.Validated = false;
            }
        }
        
        //response.Validated = true;

        return response;
    }
}