using Microsoft.AspNetCore.Mvc;

namespace CSSimulator
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargerGrainController : Controller
    {
        [HttpGet("startCharging/{index}")]
        public IEnumerable<string> start(int index)
        {
            Console.WriteLine("starts charging");
            ChargerGrainStorage.chargerGrains.ElementAt(index).status = "Starting";
            ChargerGrainStorage.chargerGrains.ElementAt(index).grain.StartCharging();
            return new string[] { "Start" };
        }
        [HttpGet("stopCharging/{index}")]
        public IEnumerable<string> stop(int index)
        {
            Console.WriteLine("stops charging");
            ChargerGrainStorage.chargerGrains.ElementAt(index).status = "Stopping";
            ChargerGrainStorage.chargerGrains.ElementAt(index).grain.StopCharging();
            return new string[] { "Stop" };
        }



    }
}
