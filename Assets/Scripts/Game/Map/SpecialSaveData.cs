namespace Game
{
    public class SpecialSaveData : MapData
    {
        public string Id;

        public SpecialSaveData(string id)
        {
            Id = id;
        }

        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }

        public override SquareInfo GetSquareInfo()
        {
            return new SquareInfo()
            {
                Name = $"square_{Id}",
                Desc = $"square_{Id}_desc"
            };
        }
    }
}