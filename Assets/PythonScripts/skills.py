from basic import *


class Skill2(PassiveSkill, CounterMaxIn, CooldownMaxIn):
    @classmethod
    def cd(cls) -> int:
        return 7

    def affect(self, timing: Timing, **kw) -> bool:
        return True

    @classmethod
    def rank(cls) -> Rank:
        return Rank(1)
