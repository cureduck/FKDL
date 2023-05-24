from enum import Enum
from sys import modules
from typing import Any

serializable = (list, str, dict, int, bool, float, set, tuple, Enum, None)


def obj2dict(obj):
    if type(obj) in serializable:
        if type(obj) in (list, set, tuple):
            return [obj2dict(o) for o in obj]
        elif isinstance(obj, (dict,)):
            return {k: obj2dict(v) for k, v in obj.items()}
        else:
            return obj
    else:
        if isinstance(obj, (list, set, tuple)):
            return dict(
                type=type(obj).__name__,
                values=[obj2dict(x) for x in obj]
            )
        dic: dict[str, Any] = dict()
        if hasattr(obj, "skip_attr"):
            skip = getattr(obj, "skip_attr")
        else:
            skip = ()
        for attr in obj.__dict__:
            if attr not in skip:
                dic[attr] = obj2dict(getattr(obj, attr))
        return dict(
            type=type(obj).__name__,
            values=dic
        )


def dict2obj(obj):
    if isinstance(obj, dict):
        if "type" in obj and "values" in obj:
            for module in modules.values():
                if hasattr(module, obj["type"]):
                    ty = getattr(module, obj["type"])
                    if isinstance(obj["values"], dict):
                        obj["values"] = dict2obj(obj["values"])
                        return ty(**obj["values"])
                    elif isinstance(obj["values"], (list, set)):
                        return ty(*[dict2obj(x) for x in obj["values"]])
        else:
            for k, v in obj.items():
                obj[k] = dict2obj(v)
                return obj
            return obj
    elif isinstance(obj, (list, set, tuple)):
        for o in obj:
            o = dict2obj(o)
        return obj
    else:
        return obj
