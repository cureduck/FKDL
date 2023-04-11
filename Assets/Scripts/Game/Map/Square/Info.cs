using Managers;

namespace Game
{
    public class Info
    {
        public virtual void BroadCastInfo()
        {
        }
    }

    public class SuccessInfo : Info
    {
        
    }

    public class FailureInfo : Info
    {
        public FailureInfo(FailureReason reason, bool autoBroadCast = true)
        {
            Reason = reason;
            if (autoBroadCast)
            {
                BroadCastInfo();
            }
        }
        public FailureReason Reason { get; }

        public sealed override void BroadCastInfo()
        {
            HelpInfoManager.Instance.SetTerm(Reason.ToString());
        }

        public override string ToString()
        {
            return Reason.ToString();
        }
    }

    public enum FailureReason
    {
        NotEnoughHp,
        NotEnoughMp,
        NotEnoughGold,
        NotEnoughSkillSlot,
        SkillAlreadyMax,
        SkillNotReady,
        SkillPassive,
        NoTarget,
        
        PotionAlreadyFull,
        SkillSlotAlreadyFull
    }
    
}