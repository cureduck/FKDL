using Managers;

namespace Game
{
    public class StairsSaveData : MapData
    {
        public string Destination;

        public StairsSaveData(string destination)
        {
            Destination = destination;
        }

        public override void OnReact()
        {
            base.OnReact();
            GameManager.Instance.LoadFloor(GameManager.Instance.Map.Floors[Destination]);
            GameManager.Instance.Map.CurrentFloor = Destination;

            if (!Destination.StartsWith("A"))
            {
                GameManager.Instance.PlayerData.March(Destination);
            }
        }
    }
}