using System;
using System.Linq;
using Managers;
using Sirenix.Utilities;

namespace Game.PlayerCommands
{
    public abstract class Command<TReceiver>
    {
        public abstract void Execute(TReceiver receiver);
    }

    public enum Mode
    {
        Random,
        Specified,
        Chest
    }


    public abstract class PlayerCommand : Command<PlayerData>
    {
        public int Count;
        public string Id;
        public Mode Mode;
        public Rank Rank;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">random chest or specified</param>
        /// <param name="id">specified id</param>
        /// <param name="count"></param>
        /// <param name="rank"></param>
        protected PlayerCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal)
        {
            Mode = mode;
            Id = id;
            Count = count;
            Rank = rank;
        }

        protected PlayerCommand()
        {
            Mode = Mode.Chest;
            Id = null;
            Count = 1;
            Rank = Rank.Normal;
        }

        public static PlayerCommand[] Interpret(string s)
        {
            if (s.IsNullOrWhitespace())
            {
                return Array.Empty<PlayerCommand>();
            }

            var cmds = s.Replace(" ", "").Split('|');
            try
            {
                return cmds.Select(InterpretSingle).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Error interpreting {s}", e);
            }
        }

        private static PlayerCommand InterpretSingle(string s)
        {
            var components = s.Split(':');
            var type = components[0];
            var id_mode = components[1]; //id or mode
            var count = components.Length > 2 ? int.Parse(components[2]) : 1;
            var rank = components.Length > 3 ? (Rank)int.Parse(components[3]) : Rank.Normal;
            var mode = id_mode == "random" ? Mode.Random : id_mode == "chest" ? Mode.Chest : Mode.Specified;

            switch (type)
            {
                case "relic":
                    return new GetRelicCommand(mode, id_mode, count, rank);
                case "potion":
                    return new GetPotionCommand(mode, id_mode, count, rank);
                case "blessing":
                    return new GetBlessingCommand(mode, id_mode, count, rank);
                case "curse":
                    return new GetCurseCommand(mode, id_mode, count, rank);
                case "skill":
                    return new GetSkillCommand(mode, id_mode, count, rank);
                case "attr":
                    return new GetAttrCommand(mode, id_mode, count, rank);
                case "gold":
                    return new GetGoldCommand(int.Parse(id_mode));
                case "buff":
                    return new GetBuffCommand(mode, id_mode, count, rank);
                case "key":
                    return new GetKeyCommand((Rank)int.Parse(id_mode));
                case "jump":
                    return new JumpCommand(mode, id_mode);
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + ":" + Mode + ":" + Id + ":" + Count + ":" + Rank;
        }
    }

    public class GetSlotCommand : PlayerCommand
    {
        public GetSlotCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            receiver.AddSkillSlot();
        }
    }

    public class SkillPointCommand : PlayerCommand
    {
        public SkillPointCommand(Mode mode = Mode.Chest, string id = null, int count = 1, Rank rank = Rank.Normal) :
            base(mode, id, count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            receiver.GetSkillPoint(Count);
        }
    }


    public class JumpCommand : PlayerCommand
    {
        public JumpCommand(Mode mode = Mode.Specified, string id = null, int count = 1, Rank rank = Rank.Normal) : base(
            mode, id, count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            var crystal = CrystalManager.Instance.Lib[Id];
            WindowManager.Instance.CrystalPanel.Open((receiver, crystal, "UI_MagicCrystal_Title"));
        }
    }


    public class GetKeyCommand : PlayerCommand
    {
        public GetKeyCommand(Rank rank, int count = 1, Mode mode = Mode.Specified, string id = null) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            for (int i = 0; i < Count; i++)
            {
                receiver.TryTakeKey(Rank, out _);
            }
        }
    }


    public class GetPotionCommand : PlayerCommand
    {
        public GetPotionCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            if (Mode == Mode.Random)
            {
                var potion = PotionManager.Instance.GenerateT(Rank, receiver.LuckyChance)[0];
                for (int i = 0; i < Count; i++)
                {
                    receiver.TryTakePotion(potion.Id, out _);
                }
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    receiver.TryTakePotion(Id, out _);
                }
            }
        }
    }

    public class GetRelicCommand : PlayerCommand
    {
        public GetRelicCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            switch (Mode)
            {
                case Mode.Random:
                    var relic = RelicManager.Instance.RollT(Rank)[0];
                    receiver.TryTakeRelic(relic.Id, out _);
                    break;
                case Mode.Specified:
                    receiver.TryTakeRelic(Id, out _);
                    break;
                case Mode.Chest:
                    GameManager.Instance.RollForRelic((int)Rank);
                    break;
                default:
                    break;
            }
        }
    }

    public class GetBuffCommand : PlayerCommand
    {
        public GetBuffCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            switch (Mode)
            {
                case Mode.Random:
                    var buff = new BuffData(BuffManager.Instance.RollT(Rank)[0].Id, Count);
                    receiver.AppliedBuff(buff);
                    break;
                case Mode.Specified:
                    receiver.AppliedBuff(new BuffData(Id, Count));
                    break;
                default:
                    break;
            }
        }
    }

    public class GetBlessingCommand : GetBuffCommand
    {
        public GetBlessingCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            switch (Mode)
            {
                case Mode.Random:
                    var blessing = BuffManager.Instance.GetRandomBuffData(BuffType.Blessing);
                    receiver.AppliedBuff(blessing);
                    return;
            }

            base.Execute(receiver);
        }
    }

    public class GetCurseCommand : GetBuffCommand
    {
        public GetCurseCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            switch (Mode)
            {
                case Mode.Random:
                    var curse = BuffManager.Instance.GetRandomBuffData(BuffType.Curse);
                    receiver.AppliedBuff(curse);
                    return;
            }

            base.Execute(receiver);
        }
    }


    public class GetSkillCommand : PlayerCommand
    {
        public GetSkillCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            switch (Mode)
            {
                case Mode.Random:
                    var skill = SkillManager.Instance.GenerateT(Rank, receiver.LuckyChance)[0];
                    receiver.TryTakeSkill(skill.Id, out _);
                    break;
                case Mode.Specified:
                    receiver.TryTakeSkill(Id, out _);
                    break;
                case Mode.Chest:
                    GameManager.Instance.RollForSkill((int)Rank);
                    break;
                default:
                    break;
            }
        }
    }


    public class GetGoldCommand : PlayerCommand
    {
        public GetGoldCommand(int count, Mode mode = Mode.Random, string id = null, Rank rank = Rank.Normal) : base(
            mode, id, count, rank)
        {
        }

        public override void Execute(PlayerData receiver)
        {
            receiver.Gain(Count);
        }
    }


    public class GetAttrCommand : PlayerCommand
    {
        public BattleStatus Modify;

        public GetAttrCommand(string id, int count)
        {
            Id = id;
            Count = count;
            Modify = GetBattleStatus(id, count);
        }

        public GetAttrCommand(Mode mode, string id = null, int count = 1, Rank rank = Rank.Normal) : base(mode, id,
            count, rank)
        {
            Modify = GetBattleStatus(id, count);
        }

        private static BattleStatus GetBattleStatus(string id, int count)
        {
            switch (id)
            {
                case "curhp":
                    return new BattleStatus(curHp: count);
                case "curmp":
                    return new BattleStatus(curMp: count);
                case "maxhp":
                    return new BattleStatus(maxHp: count);
                case "maxmp":
                    return new BattleStatus(maxMp: count);
                case "patk":
                    return new BattleStatus(pAtk: count);
                case "matk":
                    return new BattleStatus(mAtk: count);
                case "mdef":
                    return new BattleStatus(mDef: count);
                default:
                    return new BattleStatus();
            }
        }

        public override void Execute(PlayerData receiver)
        {
            if (Id.StartsWith("skillslot"))
            {
                receiver.AddSkillSlot();
                return;
            }

            if (Modify.CurHp > 0 || Modify.CurMp > 0)
            {
                receiver.Heal(Modify);
                return;
            }

            if (Modify.CurHp < 0 || Modify.CurMp < 0)
            {
                receiver.Cost(Modify);
                return;
            }

            receiver.Strengthen(Modify);
        }
    }
}