namespace CSSimulator
{
    public static class ChargerGrainStorage
    {
        public static int currentChargerGrainAmounts;
        
        public static List<ChargerGrainsDTO> chargerGrains = new();


        public static void addChargerGrain(ChargerGrain grain, string name)
        {
            ChargerGrainsDTO newGrain = new();
            newGrain.grain = grain;
            newGrain.name = name;
            chargerGrains.Add(newGrain);

        }

        internal static void UpdateLastMessage(String msg, int index)
        {
            chargerGrains[index].lastMessage = msg;
        }

        public class ChargerGrainsDTO
        {
            public ChargerGrain grain;
            public string name;
            public string status;
            public string lastMessage;
        }
        

    }
}
