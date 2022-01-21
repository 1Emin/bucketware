namespace GrowbrewProxy
{
    public class Utils
    {
        public bool isInside(int circle_x, int circle_y, int rad, int x, int y)
        {
            if ((x - circle_x) * (x - circle_x) + (y - circle_y) * (y - circle_y) <= rad * rad)
                return true;
            else
                return false;
        }
    }
    public class PlayerInfo
    {
        World worldMap = new World();
        public string PlayerName { get; set; }
        public string PlayerPassword { get; set; }
        public int PlayerNetID { get; set; }
        public string PlayerCountry { get; set; }

        public void LoadPlayerINFO()
        {
            Player playerObject = worldMap.player;
            PlayerName = Program.globalUserData.tankIDName;
            PlayerPassword = Program.globalUserData.tankIDPass;
            PlayerNetID = playerObject.netID;
            PlayerCountry = Program.globalUserData.country;
        }
    }
    public class WorldInfo
    {
        World worldMap = new World();
        public string DroppedItems { get; set; }
        public string PlayerPassword { get; set; }
        public int PlayerNetID { get; set; }
        public string PlayerCountry { get; set; }

        public void LoadPlayerINFO()
        {
            //worldMap.droppedItems
        }
    }
}
