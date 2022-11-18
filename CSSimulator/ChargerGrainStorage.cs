using System.Collections.Concurrent;

namespace CSSimulator
{
    public static class ChargerGrainStorage
    {
        public static int currentChargerGrainAmounts;
        
        public static ConcurrentQueue<ChargerGrainsDTO> chargerGrains = new();

        public static void addChargerGrain(ChargerGrain grain, string identity)
        {
            ChargerGrainsDTO newGrain = new();
            newGrain.grain = grain;
            newGrain.identity = identity;
            chargerGrains.Enqueue(newGrain);
        }

        internal static void UpdateLastMessage(String msg, int index)
        {
            try
            {
                chargerGrains.ElementAt(index).lastMessage = msg;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Couldnt save last message of chargergrain with index " + index);
            }
        }

        internal static void UpdateStatus(String status, int index)
        {
            try
            {
                chargerGrains.ElementAt(index).status = status;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Couldnt save status of chargergrain with index " + index);
            }
        }

        public class ChargerGrainsDTO
        {
            public ChargerGrain grain;
            public string identity;
            public string status;
            public string lastMessage;
        }
        

    }
}
