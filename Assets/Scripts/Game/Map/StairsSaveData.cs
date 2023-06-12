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
            if (IsNextFloor(Destination))
            {
                Player.March(Destination);
            }

            GameManager.Instance.LoadFloor(GameManager.Instance.Map.Floors[Destination]);
            GameManager.Instance.Map.CurrentFloor = Destination;
        }


        private bool IsNextFloor(string destination)
        {
            if (int.TryParse(destination, out _) &&
                int.TryParse(GameManager.Instance.Map.CurrentFloor, out _))
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