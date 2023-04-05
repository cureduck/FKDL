using Boo.Lang;
using Managers;

namespace Game
{
    public class Crystal : CsvData
    {
        public string Title;
        public List<Option> Options;

        public Crystal(Rank rank) : base(rank)
        {
            Options = new List<Option>();
        }
        
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
    }
}