import csv


def auto_split(source_path, data_path, loc_path):
    reader, f = get_csv(source_path)
    write_data(reader, f, data_path)
    reader, f = get_csv(source_path)
    write_loc(reader, f, loc_path)


def get_csv(path):
    f = open(path)
    reader = csv.DictReader(f)
    return reader, f


def write_data(reader, f, save_path):
    with open(save_path, "w", newline="") as F:
        L = []
        for kw in reader.fieldnames:
            if not ["chinese", "english", "desc_chinese", "desc_english"].__contains__(kw):
                L.append(kw)

        writer = csv.DictWriter(F, L)
        writer.writeheader()
        for row in reader:
            dic = dict([(k, row[k]) for k in L])
            writer.writerow(dic)
        f.close()


def line_converter(line):
    return line.replace("P1", "<color=yellow>{[P1]}</color>").replace("P2", "<color=yellow>{[P2]}</color>").replace(
        "Lv", "<color=yellow>Lv</color>")


def write_loc(reader, f, save_path):
    conv = line_converter
    with open(save_path, "w", newline="") as F:
        writer = csv.DictWriter(F, ["Key", "Type", "Desc", "Chinese", "English"])
        writer.writeheader()
        for row in reader:
            writer.writerow(dict(Key=row["id"].lower(), Type="", Desc="", Chinese=conv(row["chinese"]),
                                 English=conv(row["english"])))
            writer.writerow(
                dict(Key=row["id"].lower() + "_desc", Type="", Desc="", Chinese=conv(row["desc_chinese"]),
                     English=conv(row["desc_english"])))
        f.close()


auto_split(r"../../StreamingAssets/SkillsAll.csv", r"../../StreamingAssets/Skills.csv", "../skills_loc.csv")
auto_split(r"../../StreamingAssets/RelicsAll.csv", r"../../StreamingAssets/Relics.csv", "../relics_loc.csv")
