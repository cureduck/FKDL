from sys import modules

from basics import *

serializable = (list, str, dict, int, bool, float, set, tuple, Enum)


def obj2dict(obj):
    if isinstance(obj, serializable):
        if isinstance(obj, (list, set, tuple)):
            return [obj2dict(o) for o in obj]
        elif isinstance(obj, (dict,)):
            return {k: obj2dict(v) for k, v in obj.items()}
        else:
            return obj
    else:
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
            for module in modules:
                if hasattr(module, obj["type"]):
                    ty = getattr(module, obj["type"])
                    return ty(**obj["values"])
        else:
            return obj
    else:
        return obj
