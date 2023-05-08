using System.Collections.Generic;
using Managers;

namespace Game
{
    /// <summary>
    /// 没消息就是好消息，空消息即为成功消息
    /// </summary>
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
            Reason = new List<FailureReason>() { reason }; //{reason};
            if (autoBroadCast)
            {
                BroadCastInfo();
            }
        }

        public FailureInfo(List<FailureReason> reason, bool autoBroadCast = true)
        {
            Reason = reason;
            if (autoBroadCast)
            {
                BroadCastInfo();
            }
        }

        public List<FailureReason> Reason { get; }

        public sealed override void BroadCastInfo()
        {
            HelpInfoManager.Instance.SetTerm(Reason[0].ToString());
        }

        public override string ToString()
        {
            return Reason[0].ToString();
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