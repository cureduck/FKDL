from .basics import *


class SupplyMapData(MapData):
    def react(self, player: Player):
        player.heal(10)


class StartMapData(MapData):
    def react(self, player):
        pass
