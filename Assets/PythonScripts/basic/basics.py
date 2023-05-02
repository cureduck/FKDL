from __future__ import annotations

from abc import abstractmethod
from enum import *
from typing import Sequence, Any, NoReturn, Union, Optional


@unique
class State(Enum):
    Done = 0
    Focus = 1
    UnFocus = 2
    Revealed = 3


class MaxIn:
    default_init = dict[str, Any]()


@unique
class Timing(Enum):
    OnSetCd = 0
    OnAttack = 1


class BattleStatus:
    def __init__(self, cur_hp=0, cur_mp=0, max_hp=0, max_mp=0, p_atk=0, m_atk=0, p_def=0, m_def=0, coin=0):
        self.cur_hp = cur_hp
        self.cur_mp = cur_mp
        self.max_hp = max_hp
        self.max_mp = max_mp
        self.p_atk = p_atk
        self.m_atk = m_atk
        self.p_def = p_def
        self.m_def = m_def
        self.coin = coin


Location = (int, int, int, int)


class MapData:
    def __init__(self, location: Location, _state):
        self.location = tuple(location)
        self._state = _state

    @property
    def state(self) -> State:
        return State(self._state)

    @state.setter
    def state(self, value: (int, State)):
        if isinstance(value, int):
            self._state = value
        elif isinstance(value, State):
            self._state = value.value

    def destroy(self):
        pass

    @abstractmethod
    def react(self, player: Player):
        raise NotImplementedError()

    def reveal_around(self, m: Sequence[MapData]):
        for sq in m:
            if sq._state == 0 and sq.next_to(self.location):
                sq._state = 1

    def next_to(self, p2: Location) -> bool:
        x1, y1, w1, h1 = self.location
        x2, y2, w2, h2 = p2
        p3 = (
            max(x1, x2),
            max(y1, y2),
            min(x1 + w1, x2 + w2) - max(x1, x2),
            min(y1 + h1, y2 + h2) - max(y1, y2),
        )
        x3, y3, w3, h3 = p3
        return not (h3 < 0 or w3 < 0) and (not ((h3 == 0) and (w3 == 0)))


class Rank(Enum):
    Common = 0
    Uncommon = 1
    Rare = 2


class RankMaxIn(MaxIn):
    """
    品阶组件
    """

    rank = Rank(0)


class Attack:
    __slots__ = ("patk", "matk", "catk", "pdmg", "mdmg", "cdmg", "multi", "combo", "id", "kw")

    def __init__(self, patk: int = 0, matk: int = 0, catk: int = 0, pdmg: int = 0, cdmg: int = 0, mdmg: int = 0,
                 multi: float = 1, combo: int = 1, id: str = None, kw: str = None):
        self.patk = patk
        self.matk = matk
        self.catk = catk
        self.pdmg = pdmg
        self.mdmg = mdmg
        self.cdmg = cdmg
        self.multi = multi
        self.combo = combo
        self.id = id
        self.kw = kw


class Factor:
    def affect(self, timing: Timing, **kw: Union[Attack, Player, Enemy, _Skill]) -> bool:
        """
        技能、遗物、buff
        :param timing: 时机
        :param kw: caster、enemy、attack、skill
        :return:
        """
        raise NotImplementedError(self)

    @classmethod
    def priority(cls, timing: Timing) -> Optional[int]:
        """
        :param timing 时机
        :return: 返回值为数值即为优先级，若为None则为不影响，默认返回0
        """
        return 0


class _SkillMetaclass(type):
    def __new__(cls, name, bases, attrs):
        if "init" in attrs:
            pass
        else:
            init = dict()
            for base in bases:
                if issubclass(base, MaxIn):
                    init.update(base.default_init)
            attrs["init"] = init
        return type.__new__(cls, name, bases, attrs)


@unique
class Prof(Enum):
    Common = 0,
    Assassin = 1,
    Alchemist = 2,
    Mage = 3


class ProfMaxIn(MaxIn):
    prof = Prof.Common


class _Skill(RankMaxIn, ProfMaxIn, Factor, metaclass=_SkillMetaclass):
    activated = True

    def upgrade(self):
        self.cur_lv += 1

    def __init__(self, cur_lv: int = 1, **kw):
        super().__init__()
        self.cur_lv = cur_lv
        for k, v in self.init.items():
            setattr(self, k, v)
        for k, v in kw.items():
            setattr(self, k, v)


class SkillAgent(list[_Skill, None]):
    def __init__(self, *members):
        super().__init__(members)

    def upgrade_random(self):
        raise NotImplementedError()


class Buff(Factor):
    def __init__(self, cur_lv):
        self.cur_lv = cur_lv


class BuffAgent(list[Buff]):
    def __init__(self, *members):
        super().__init__(members)


class FightMaxIn:
    def __init__(self, status: BattleStatus, skills: SkillAgent, buffs: BuffAgent, cloned: bool = False):
        self.status = status
        self.cloned = cloned
        self.skills = skills
        self.buffs = buffs

    @property
    def alive(self) -> bool:
        return self.status.cur_hp > 0

    def heal(self, value: int, kw: str = None) -> NoReturn:
        pass

    @abstractmethod
    def get_factors(self) -> Sequence[Factor]:
        pass

    def modify(self, timing: Timing, **kw):
        factors = filter(lambda x: x.priority is not None, self.get_factors())
        s = sorted(
            factors,
            key=lambda x: x.priority
        )
        for fs in s:
            fs.affect(timing, **kw)


class CounterMaxIn(MaxIn):
    default_init = {"counter": 0}

    def __init__(self, counter=0):
        self.counter = counter


class CooldownMaxIn(MaxIn):
    default_init = {"cd_init": int(), "cd_left": int()}

    def __init__(self, cd_init: int = 0, cd_left: int = 0):
        self.cd_init = cd_init
        self.cd_left = cd_left

    cd = 7


class PassiveSkill(_Skill):

    @classmethod
    def rank(cls) -> Rank:
        pass


class _ActiveSkill(_Skill):
    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        """
        主动技能施放
        :param caster: 施法者
        :param target: 敌人
        :return:若是战斗回合，则返回attack，否则则是None
        """
        pass


class AimingSKill(_ActiveSkill):

    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        pass


class NonAimingSKill(_ActiveSkill):

    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        pass


class Player(FightMaxIn):

    def get_factors(self) -> Sequence[Factor]:
        l: list[Factor] = self.skills.copy()
        l.extend(self.buffs)
        return l

    skip_attr = ("cloned",)

    def __init__(self, status, skills: SkillAgent, buffs: BuffAgent):
        super().__init__(status, skills, buffs)


class Enemy(MapData, FightMaxIn):

    def react(self, player):
        pass

    def get_factors(self) -> Sequence[Factor]:
        l: list[Factor] = self.skills.copy()
        l.extend(self.buffs)
        return l

    skip_attr = ("cloned",)

    def __init__(self, status: BattleStatus, coord: (int, int), _state: int, lucky: float, cloned=False):
        super().__init__(coord, _state)
        self.status = status
        self.cloned = cloned
        self.lucky = lucky


def translator(ch, en, ch_desc="", en_desc=""):
    def wrapper(cls):
        cls.ch = ch
        cls.en = en
        cls.ch_desc = ch_desc
        cls.en_desc = en_desc
        return cls

    return wrapper
