from .basics import *


class Floor(list[MapData]):
    def __init__(self, *members):
        super().__init__(members)


FloorInfo = (str, Floor)


class Map(list[FloorInfo]):
    def __init__(self, *members):
        super().__init__(members)
