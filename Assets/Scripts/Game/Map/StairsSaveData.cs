using Managers;

namespace Game
{
    public class StairsSaveData : MapData
    {
        public string Destination;
        private bool needWait;

        public StairsSaveData(string destination)
        {
            Destination = destination;
        }

        public override async void OnReact()
        {
            if (needWait) return;
            base.OnReact();
            needWait = true;
            await System.Threading.Tasks.Task.Delay(500);
            needWait = false;
            GameManager.Instance.LoadFloor(GameManager.Instance.Map.Floors[Destination]);
            GameManager.Instance.Map.CurrentFloor = Destination;

            if (IsNextFloor(Destination))
            {
                Player.March(Destination);
            }
        }


        private bool IsNextFloor(string destination)
        {
            if (!destination.ToLower().StartsWith("a") &&
                !GameManager.Instance.Map.CurrentFloor.ToLower().StartsWith("a"))
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