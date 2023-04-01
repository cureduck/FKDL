using Boo.Lang;

namespace Game
{
    public class Crystal : IRank
    {
        public string Id;
        public string Title;
        public List<Option> Options;

        public Crystal()
        {
            Options = new List<Option>();
        }
        
        public class Option
        {
            public string Effect;
            public string Line;
            public CostInfo CostInfo;
        }

        public Rank Rank { get; set; }
    }
}