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

            if (IsNextFloor(Destination))
            {
                GameManager.Instance.Player.March(Destination);
            }
        }

        private bool IsNextFloor(string destination)
        {
            if (!destination.StartsWith("A") && !GameManager.Instance.Map.CurrentFloor.StartsWith("A"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}