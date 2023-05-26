import json

from basic import *
from typing import Optional


@translator(ch="测试被动技能", en="test passive skill")
class TestSkill(PassiveSkill, CounterMaxIn, CooldownMaxIn):
    cd = 5
    rank = Rank(1)
    prof = Prof.Mage

    def affect(self, timing: Timing, **kw: Union[Attack, Player, Enemy]) -> bool:
        if timing != Timing.OnAttack:
            return False
        if self.counter < 0:
            self.counter += 1
        else:
            (kw["Attack"]).patk += 1
            self.counter = 0
        return True

    @classmethod
    def priority(cls, timing: Timing) -> Optional[int]:
        if timing != Timing.OnAttack:
            return None
        else:
            return 1


@translator(ch="测试主动技能", en="test passive skill")
class Test2Skill(AimingSKill, CounterMaxIn):
    """
    测试主动技能，效果为计数器满时，物攻倍率+1
    """

    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        return Attack(patk=caster.status.p_atk, multi=2)

    rank = Rank(1)
    prof = Prof.Mage

    def affect(self, timing: Timing, **kw: Union[Attack, Player, Enemy]) -> bool:
        if timing != Timing.OnAttack:
            return False
        if self.counter < 0:
            self.counter += 1
        else:
            (kw["Attack"]).patk += 1
            self.counter = 0
        return True

    @classmethod
    def priority(cls, timing: Timing) -> Optional[int]:
        if timing != Timing.OnAttack:
            return None
        else:
            return 1


player = Player(BattleStatus(), SkillAgent(TestSkill()), BuffAgent())

atk = Attack()
player.modify(timing=Timing.OnAttack, Attack=atk)

print(atk)
floor = Floor(
    SupplyMapData((1, 2, 1, 1), 0),
    SupplyMapData((1, 1, 1, 1), 0),
    SupplyMapData((1, 1, 3, 1), 1),
    SupplyMapData((1, 1, 1, 4), 2),
)

m = Map(("tt.__str__()", floor))

s = json.dumps(obj2dict(m), indent="    ")
print(s)

mm = json.loads(s)

s = SupplyMapData((1, 1, 1, 1), 0)
s.reveal_around(floor)

ss = (dict2obj(obj2dict(m)))
print(obj2dict(ss))


class student:
    def __init__(self, name, age):
        self.name = name
        self.age = age

a = student("a", 1)
a_str = json.dumps(obj2dict(a))
b_dict = json.loads(a_str)
b = dict2obj(b_dict)
print(b.name)