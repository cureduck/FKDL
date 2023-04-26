import json
from abc import abstractmethod
from enum import *
from struct import Struct
from typing import Sequence, NoReturn, Any


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


class MapData:
    def __init__(self, coord: (int, int), _state):
        self.coord = coord
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

    def react(self):
        raise NotImplementedError

    def __reveal_around__(self):
        pass


class Rank(Enum):
    Common = 0
    Uncommon = 1
    Rare = 2


class RankMaxIn(MaxIn):
    """
    品阶组件
    """

    @classmethod
    @abstractmethod
    def rank(cls) -> Rank:
        pass


class Attack:
    __slots__ = ("patk", "matk", "catk", "pdmg", "mdmg", "cdmg", "multi", "combo", "id", "kw")

    def __init__(self, patk: int, matk: int, catk: int, pdmg: int, cdmg: int, mdmg: int, multi: float = 1,
                 combo: int = 1, id: str = None, kw: str = None):
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
    def affect(self, timing: Timing, **kw) -> bool:
        """
        技能、遗物、buff
        :param timing: 时机
        :param kw: caster、enemy、attack、skill
        :return:
        """
        raise NotImplementedError(self)


class SkillMetaclass(type):
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


class Skill(RankMaxIn, Factor, metaclass=SkillMetaclass):
    @abstractmethod
    def rank(self) -> Rank:
        pass

    def __init__(self, cur_lv: int = 1, **kw: [str, Any]):
        super().__init__()
        self.cur_lv = cur_lv
        for k, v in self.init.items():
            setattr(self, k, v)
        for k, v in kw.items():
            setattr(self, k, v)


class SkillAgent(list[Skill, None]):
    def __init__(self):
        super().__init__()


class Buff(Factor):
    def __init__(self, cur_lv):
        self.cur_lv = cur_lv


class BuffAgent(list[Buff]):
    pass


class FightMaxIn:
    def __init__(self, status, skills: SkillAgent, buffs: BuffAgent, cloned: bool = False):
        self.status = status
        self.cloned = cloned
        self.skills = skills
        self.buffs = buffs

    @property
    def alive(self) -> bool:
        return self.status.curhp > 0

    def heal(self, value: int, kw: str):
        pass

    @abstractmethod
    def get_factors(self) -> Sequence[Sequence[Factor]]:
        pass

    def modify(self, timing: Timing, **kw):
        factors = self.get_factors()
        for fs in factors:
            for f in fs:
                f.affect(timing, **kw)


class CounterMaxIn(MaxIn):
    default_init = {"counter": 0}

    def __init__(self, counter=0):
        self.counter = counter


class CooldownMaxIn(MaxIn):
    default_init = {"cd_init": int(), "cd_left": int()}

    def __init__(self, cd_init: int = 0, cd_left: int = 0):
        self.cd_init = cd_init
        self.cd_left = cd_left

    @classmethod
    @abstractmethod
    def cd(cls) -> int:
        pass

    def set_cd(self, factors: Sequence[Factor]):
        for factor in factors:
            factor.affect(timing=Timing.OnSetCd, skill=self)


class PassiveSkill(Skill):

    @classmethod
    def rank(cls) -> Rank:
        pass


class ActiveSkill(Skill):
    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        """
        主动技能施放
        :param caster: 施法者
        :param target: 敌人
        :return:若是战斗回合，则返回attack，否则则是None
        """
        pass


class AimingSKill(ActiveSkill):

    @classmethod
    @abstractmethod
    def rank(cls) -> Rank:
        pass

    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        pass


class NonAimingSKill(ActiveSkill):

    @classmethod
    @abstractmethod
    def rank(cls) -> Rank:
        pass

    @abstractmethod
    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        pass


class Player(FightMaxIn):

    def get_factors(self) -> Sequence[Sequence[Factor]]:
        return self.skills, self.buffs

    skip_attr = ("cloned",)

    def __init__(self, status, skills: SkillAgent, buffs: BuffAgent):
        super().__init__(status, skills, buffs)


class Enemy(MapData, FightMaxIn):

    def get_factors(self) -> Sequence[Sequence[Factor]]:
        return self.skills, self.buffs

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


@translator("sdf", "sdf")
class Test1Skill(AimingSKill, CounterMaxIn, CooldownMaxIn):

    @classmethod
    def rank(cls) -> Rank:
        return Rank(0)

    def cast(self, caster: FightMaxIn, target: (FightMaxIn, None)) -> (Attack, None):
        pass

    def affect(self, timing, **kw: [str, (Attack, Enemy, Player)]) -> bool:
        (kw["attack"]).patk += 1
        return True

    @classmethod
    def cd(cls) -> int:
        return 7
