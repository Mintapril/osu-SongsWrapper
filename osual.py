import os
import shutil
import re
def startwrapper(packname, artist, creator, hp, od):
    if packname:
        pass
    else:
        packname = "Collection 1st"
    if artist:
        pass
    else:
        artist = "Various"
    if creator:
        pass
    else:
        creator = ""
    judgment = ""
    judgment = hp.isdigit()
    if judgment is False or int(hp) > 10:
        hp = "9"
    judgment = ""
    judgment = od.isdigit()
    if judgment is False or int(od) > 10:
        od = "7.5"
    path = os.path.abspath('.')
    ###########################################################################
    ####读取所有osu文件信息###################
    #######################################
    osufilespath = []  ######所有osu文件的列表
    audiofilespath = []
    for p, l, osufilenames in os.walk(".", topdown=False):
        for osufiles in osufilenames:
            if ".osu" in osufiles:
                for f in osufilenames:
                    fn = os.path.join(p, f)
                    if f == osufiles:
                        osufilespath.append(os.path.abspath(fn))
        for osufiles in osufilenames:
            if ".osu" in osufiles:
                for f in osufilenames:
                    fn = os.path.join(p, f)
                    if f == osufiles:
                        audiofilespath.append(os.path.abspath(fn))
    if len(audiofilespath) == 0:
        raise FileNotFoundError("No osu file found!")
    if len(audiofilespath) > 32:
        print("Too many songs, please wait......")
    number = len(osufilespath)
    audiofilename = ""
    audioformat = ""
    backgroundformat = ""
    bg = ""
    artistname = ""
    songname = ""
    diff = ""
    os.makedirs(".tmp", exist_ok=True)  #新建临时文件夹，用于存放将要打包的文件
    while number - 1 >= 0:
        osufile = open(osufilespath[number - 1], "r+", errors='ignore')
        for line in osufile:
            if "AudioFilename" in line:
                audiofilename = line
                audiofilename = audiofilename.replace("AudioFilename: ", "")
                audiofilename = audiofilename.strip("\n")
            if "Title:" in line:
                songname = line
                songname = songname.replace("Title:", "")
                songname = songname.strip("\n")
                if songname == "":
                    songname = "Undefined"
                if "\\" or "/" or ":" or "?" or '"' or "<" or ">" or "|" or "*" in songname:
                    songname = songname.replace("\\", "").replace(
                        "/", "").replace(":", "").replace("?", "").replace(
                            '"', "").replace("<", "").replace(">", "").replace(
                                "|", "").replace("*", "")
            if "Artist:" in line:
                artistname = line
                artistname = artistname.replace("Artist:", "")
                artistname = artistname.strip("\n")
                if artistname == "":
                    artistname = "Undefined"
                if "\\" or "/" or ":" or "?" or '"' or "<" or ">" or "|" or "*" in artistname:
                    artistname = artistname.replace("\\", "").replace(
                        "/", "").replace(":", "").replace("?", "").replace(
                            '"', "").replace("<", "").replace(">", "").replace(
                                "|", "").replace("*", "")
            if "Version:" in line:
                diff = line
                diff = line.replace("Version:", "")
                diff = diff.strip("\n")
                if "\\" or "/" or ":" or "?" or '"' or "<" or ">" or "|" or "*" in diff:
                    diff = diff.replace("\\", "").replace("/", "").replace(
                        ":", "").replace("?", "").replace('"', "").replace(
                            "<", "").replace(">", "").replace("|", "").replace(
                                "*", "")
            if '0,0,"' in line:
                bg = line
                bg = bg.replace('0,0,"', "")
                if '",0,0' in bg:
                    bg = bg.replace('",0,0', "")
                if '"' in bg:
                    bg = bg.replace('"', "")
                bg = bg.strip("\n")
            if ".jpg" in line:
                backgroundformat = ".jpg"
            if ".png" in line:
                backgroundformat = ".png"
            if ".mp3" in line:
                audioformat = ".mp3"
            if ".ogg" in line:
                audioformat = ".ogg"
        audio = artistname + " - " + songname + " [" + diff + "] " + audioformat
        background = artistname + " - " + songname.replace(".", "").replace(",", "") + " [" + diff + "] " + backgroundformat  #新的background名字
        version = artistname + " - " + songname + " [" + diff + "]"  #难度名称的地方标注上歌曲和做曲者名字和难度
        pathnewosufiles = os.path.join(
            ".tmp", artist + " - " + packname + " (" + creator + ")" + " [" +
            version + "]" + ".osu")  #新的osu文件的路径（临时）
        osufile.close()
        f3 = open(pathnewosufiles, "w")
        f2 = open(osufilespath[number - 1], "r+", errors='ignore')
        for line in f2:
            #开始修改osu文件......
            if "AudioFilename:" in line:
                line = re.sub(r"AudioFilename: .*", "AudioFilename: " + audio,
                              line)
            if "Title:" in line:
                line = re.sub(r"Title:.*", "Title:" + packname, line)
            if "TitleUnicode:" in line:
                line = re.sub(r"TitleUnicode:.*", "TitleUnicode:" + packname,
                              line)
            if "Artist:" in line:
                line = re.sub(r"Artist:.*", "Artist:" + artist, line)
            if "ArtistUnicode:" in line:
                line = re.sub(r"ArtistUnicode:.*", "ArtistUnicode:" + artist,
                              line)
            if "Creator:" in line:
                line = re.sub(r"Creator:.*", "Creator:" + creator, line)
            if "Version:" in line:
                line = re.sub(r"Version:.*", "Version:" + version, line)
            if "BeatmapID:" in line:
                line = re.sub(r"BeatmapID:.*", "BeatmapID:0", line)
            if "BeatmapSetID:" in line:
                line = re.sub(r"BeatmapSetID:.*", "BeatmapSetID:-1", line)
            if "HPDrainRate:" in line:
                line = re.sub(r"HPDrainRate:.*", "HPDrainRate:" + hp, line)
            if "OverallDifficulty:" in line:
                line = re.sub(r"OverallDifficulty:.*",
                              "OverallDifficulty:" + od, line)
            if "0,0," in line:
                line = re.sub(r'0,0,.*"', '0,0,"' + background + '",0,0', line)
            f3.write(line)
        f2.close()
        f3.close()
        p, f = os.path.split(osufilespath[number - 1])
        try:
            shutil.copyfile(
                os.path.join(p, audiofilename), os.path.join(path, ".tmp", audio))
        except FileNotFoundError as e:
            print(e)
            print("No audio file found, igloned.")
            os.remove(pathnewosufiles)
        try:
            shutil.copyfile(
                os.path.join(p, bg), os.path.join(path, ".tmp", background))
        except FileNotFoundError as e:
            print(e)
            print("No background file found, igloned.")
        number = number - 1
    #复制完成
    #开始打包所有临时文件为osz
    shutil.make_archive(artist + " - " + packname, 'zip',
                        os.path.join(path, ".tmp"))
    os.rename(
        os.path.join(path, artist + " - " + packname + '.zip'),
        os.path.join(path, artist + " - " + packname + '.osz'))
    #打包完成
    #删除临时文件夹
    for folder, subfolders, files in os.walk(os.path.join(path, ".tmp")):
        for allfile in files:
            os.remove(os.path.join(path, ".tmp", allfile))
    os.rmdir(os.path.join(path, ".tmp"))
    print("Packaged successfully!")
