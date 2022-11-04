using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

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
            ChargerGrainStorage.chargerGrains.ElementAt(index)/*chargerGrains[index]*/.status = "Starting";
            ChargerGrainStorage.chargerGrains.ElementAt(index)/*chargerGrains[index]*/.grain.StartCharging();
            return new string[] { "Start" };
        }
        [HttpGet("stopCharging/{index}")]
        public IEnumerable<string> stop(int index)
        {
            Console.WriteLine("stops charging");
            ChargerGrainStorage.chargerGrains.ElementAt(index)/*chargerGrains[index]*/.status = "Stopping";
            ChargerGrainStorage.chargerGrains.ElementAt(index)/*chargerGrains[index]*/.grain.StopCharging();
            return new string[] { "Stop" };
        }


        //[HttpGet]
        //public ActionResult GetMessage()
        //{
        //    Console.WriteLine("IDKIDK");
        //    resp res = new resp();
        //    res.data = "AA";
        //    return new JsonResult(res);
        //}
        //public class resp
        //{
        //    public string data;
        //}
    }
}
