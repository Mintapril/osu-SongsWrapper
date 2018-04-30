using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;


namespace MapCollator
{
    public class MutiThreading
    {
        public static void StartMutiThreading()
        {
            System.Threading.Thread t1 = new System.Threading.Thread(App.distribute);
            System.Threading.Thread t2 = new System.Threading.Thread(App.distribute);
            System.Threading.Thread t3 = new System.Threading.Thread(App.distribute);
            System.Threading.Thread t4 = new System.Threading.Thread(App.distribute);
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();

            t1.Abort();
            t2.Abort();
            t3.Abort();
            t4.Abort();
        }
    }
    
    public class GlobalValue
    {
        public static string path;
    }
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string path, folderName, packName, artists, creator, HP, OD;
        private static Dictionary<string, bool> allFileDict = new Dictionary<string, bool>();
        private static List<string> allFileList = IO.allFileList;

        public class Program
        {
            public static double od;
            public static double hp;
            public static void ShowErrorMessageBox()
            {
                MessageBox.Show("The value of OD or HP must be an integer or floating-point number and must be between 0 and 10.", "Error");
            }

            public static void Start()
            {

                //创建新文件夹存放生成的新文件
                folderName = Path.Combine(Path.Combine(path.Replace(Path.GetFileName(path), ""), String.Format("{0}{1}{2}", artists, " - ", packName)));
                Directory.CreateDirectory(folderName);

                if (path != String.Empty)
                {
                    IO.GetFileList(path);
                    foreach (var item in IO.allFileList)
                    {
                        if (allFileDict.ContainsKey(item) == false)
                        {
                            allFileDict.Add(item, false);
                        }
                    }
                    IO.folderName = folderName;
                    IO.packName = packName;
                    IO.artists = artists;
                    IO.creator = creator;
                    IO.HP = HP;
                    IO.OD = OD;

                    MutiThreading.StartMutiThreading();


                    //打包到osz
                    IO.PackToOsz(folderName);
                    //删除临时文件夹
                    Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName), true);
                    MessageBox.Show("Packaged successfully!");
                    allFileDict.Clear();
                    allFileList.Clear();
                    StructuralAnalysis.mainDict.Clear();
                    StructuralAnalysis.opt.Clear();


        }
    }
        }
        //循环读取每个osu文件以及修改
        public static void distribute()
        {
            //线程同步
            lock(allFileDict)
            {
                foreach (string item in allFileList)
                {
                    if (item.Contains(".osu"))
                    {
                        if (allFileDict[item] == false)
                        {
                            IO.path = item;

                            IO.ReadFile();
                            allFileDict[item] = true;

                        }
                    }
                }
            }
        }
    }
    class IO
    {
        public static string path, folderName, packName, artists, creator, HP, OD;
        private static string line = null, title = null, artist = null, version = null, audioFileName = null, backgroundFileName = null;
        private static string newVersion = null, newAudioFileName = null, newBgFileName = null;
        private static string newFilePath = null;
        private static string pathWithOutFileName;
        private static string audioFileFormat, backgroundFileFormat;
        private static string oldAudioFilePath;
        private static string oldBgFilePath;
        private static string newAudioFilePath;
        private static string newBgFilePath;

        static string opt_a, opt_b;
        public static void ReadFile()
        {

            pathWithOutFileName = path.Replace(Path.GetFileName(path), "");
            using (StreamReader f1 = new StreamReader(path))
            {
                while ((line = f1.ReadLine()) != null)
                {
                    if (line.Contains("Title:"))
                    {
                        title = line.Replace("Title:", "");
                    }
                    if (line.Contains("Artist:"))
                    {
                        artist = line.Replace("Artist:", "");
                    }
                    if (line.Contains("Version:"))
                    {
                        version = line.Replace("Version:", "");
                        newVersion = String.Format("{1}{3}{0}{4}{2}{5}", title, artist, version, " - ", " [", "] ");
                    }
                    if (newVersion != null)
                    {
                        break;
                    }
                }
                //过滤掉文件名中的非法字符
                newFilePath = FilterIllegalChars(String.Format("{1}{3}{0}{4}{2}{5}{7}{6}{8}{9}", packName, artists, newVersion, " - ", " [", "] ", creator, "(", ")", ".osu"));
            }
            //有关判断是否一首歌是否有不同音频和背景的逻辑
            if (StructuralAnalysis.opt.ContainsKey(path))
            {
                if (StructuralAnalysis.opt[path] == null)
                {
                    opt_a = "";
                    opt_b = "";
                }
                if (StructuralAnalysis.opt[path][0] == "all")
                {
                    opt_a = opt_b = StructuralAnalysis.opt[path][1];
                }
                else if (StructuralAnalysis.opt[path][0] == "audio")
                {
                    opt_a = StructuralAnalysis.opt[path][1];
                    opt_b = "";
                }
                else if (StructuralAnalysis.opt[path][0] == "bg")
                {
                    opt_b = StructuralAnalysis.opt[path][1];
                    opt_a = "";
                }
            }
            else
            {
                opt_a = "";
                opt_b = "";
            }

            //读取文件
            using (StreamReader f = new StreamReader(path))
            {
                //创建新文件
                using (StreamWriter fw = new StreamWriter(Path.Combine(GlobalValue.path, folderName, newFilePath)))
                {
                    while ((line = f.ReadLine()) != null)
                    {
                        if (line.Contains("AudioFilename:"))
                        {
                            audioFileName = line.Replace("AudioFilename: ", "");
                            audioFileFormat = Path.GetExtension(audioFileName);
                            //新的音频文件名字
                            line = FilterIllegalChars_2(String.Format("{4}{0}{1}{2}{5}{3}", artist, " - ", title, audioFileFormat, "AudioFilename: ", opt_a));
                            newAudioFileName = String.Format("{0}{1}{2}{4}{3}", artist, " - ", title, audioFileFormat, opt_a);
                            newAudioFileName = FilterIllegalChars_2(newAudioFileName);
                        }
                        if (line.Contains("0,0,\"") && line.Contains("\",0,0"))
                        {
                            backgroundFileName = line.Replace("0,0,\"", "").Replace("\",0,0", "");
                            backgroundFileFormat = Path.GetExtension(backgroundFileName);
                            //新的bg文件名字
                            line = String.Format("{4}{0}{1}{2}{6}{3}{5}", FilterIllegalChars_2(artist), " - ", FilterIllegalChars_2(title.TrimEnd('.')), backgroundFileFormat, "0,0,\"", "\",0,0", opt_b);
                            newBgFileName = String.Format("{0}{1}{2}{4}{3}", artist, " - ", title, backgroundFileFormat, opt_b);
                            newBgFileName = FilterIllegalChars_2(newBgFileName);
                        }
                        if (line.Contains("Version:"))
                        {
                            version = line.Replace("Version:", "");
                            newVersion = String.Format("{1}{3}{0}{4}{2}{5}", title, artist, version, " - ", " [", "] ");
                            line = String.Format("{0}{1}", "Version:", newVersion);
                        }
                        if (line.Contains("Title:"))
                        {
                            title = line.Replace("Title:", "");
                            line = String.Format("{0}{1}", "Title:", packName);
                        }
                        if (line.Contains("TitleUnicode:"))
                        {
                            line = String.Format("{0}{1}", "TitleUnicode:", packName);
                        }
                        if (line.Contains("Artist:"))
                        {
                            artist = line.Replace("Artist:", "");
                            line = String.Format("{0}{1}", "Artist:", artists);
                        }
                        if (line.Contains("ArtistUnicode:"))
                        {
                            line = String.Format("{0}{1}", "ArtistUnicode:", artists);
                        }
                        if (line.Contains("Creator:"))
                        {
                            line = String.Format("{0}{1}", "Creator:", creator);
                        }
                        if (line.Contains("HPDrainRate:"))
                        {
                            line = String.Format("{0}{1}", "HPDrainRate:", HP);
                        }
                        if (line.Contains("OverallDifficulty:"))
                        {
                            line = String.Format("{0}{1}", "OverallDifficulty:", OD);
                        }
                        if (line.Contains("BeatmapID:"))
                        {
                            line = String.Format("{0}{1}", "BeatmapID:", "0");
                        }
                        if (line.Contains("BeatmapSetID:"))
                        {
                            line = String.Format("{0}{1}", "BeatmapSetID:", "-1");
                        }
                        //这里用来写入修改后的内容
                        fw.WriteLine(line);

                    }
                }
            }
            try
            {
                //复制所关联的音频和bg文件到新的文件夹并应用新的名字
                oldAudioFilePath = Path.Combine(pathWithOutFileName, audioFileName);
                oldBgFilePath = Path.Combine(pathWithOutFileName, backgroundFileName);
                newAudioFilePath = Path.Combine(folderName, newAudioFileName);
                newBgFilePath = Path.Combine(folderName, newBgFileName);
                File.Copy(oldAudioFilePath, newAudioFilePath);
                File.Copy(oldBgFilePath, newBgFilePath);
            }
            catch (FileNotFoundException)
            {
            }
            catch (IOException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            title = null; artist = null; version = null; audioFileName = null; backgroundFileName = null;
            newVersion = null; newAudioFileName = null; newBgFileName = null;
            newFilePath = null;

        }
    public static string[] fileList;
        public static List<string> allFileList = new List<string>();
        //遍历文件夹
        public static void GetFileList(string rootPath)
        {
            string[] pathList = Directory.GetDirectories(rootPath);
            foreach (string item in pathList)
            {
                GetPathList(item);
            }

        }
        public static string[] GetFileList(string rootPath, bool ShowPathInBox)
        {
            string[] pathList = Directory.GetDirectories(rootPath);
            return pathList;
        }

        public static void GetPathList(string item)
        {
            fileList = Directory.GetFiles(item);
            foreach (string file in fileList)
            {
                allFileList.Add(file);
            }

        }
        static string str;
        public static string FilterIllegalChars(string path)
        {
            try
            {
                str = Path.GetFileName(path);
            }
            catch (ArgumentException)
            {
            }
            str = str.Replace("\\", "_");
            str = str.Replace("/", "_");
            str = str.Replace("*", "_");
            str = str.Replace("?", "_");
            str = str.Replace("\"", "_");
            str = str.Replace("<", "_");
            str = str.Replace(">", "_");
            str = str.Replace("|", "_");
            str = str.Replace(":", "_");
            str = str.Replace("..", ".");

            return str;

        }
        public static string FilterIllegalChars_2(string path)
        {
            string str = path;
            str = str.Replace("\\", "_");
            str = str.Replace("/", "_");
            str = str.Replace("*", "_");
            str = str.Replace("?", "_");
            str = str.Replace("\"", "_");
            str = str.Replace("<", "_");
            str = str.Replace(">", "_");
            str = str.Replace("|", "_");
            str = str.Replace(":", "_");
            str = str.Replace("..", ".");
            return str;

        }
        public static void PackToOsz(string rootPath)
        {
            string Args = "-tZip a \"" + rootPath + ".osz\" \"" + rootPath + "\\*\" -mx3";
            Process pro = new Process();
            pro.StartInfo.FileName = @"Ref\7z.exe";
            pro.StartInfo.Arguments = Args;
            pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pro.Start();
            pro.WaitForExit();

        }
    }
    //预先解析文件结构方便下一行注释的内容
    //有关判断是否一首歌是否有不同音频和背景的逻辑
    public class StructuralAnalysis
    {
        public static Dictionary<string, List<string>> mainDict = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> opt = new Dictionary<string, List<string>>();
        static string line;
        static string title;
        static string artist;
        static string version;
        static string newVersion;
        static string audioFileName;
        static string backgroundFileName;

        public static string Optional;

        public static void AnalyzeStructure()
        {
            foreach (string path in IO.allFileList)
            {
                if (path.Contains(".osu"))
                {
                    using (StreamReader f1 = new StreamReader(path))
                    {
                        while ((line = f1.ReadLine()) != null)
                        {
                            if (line.Contains("Title:"))
                            {
                                title = line.Replace("Title:", "");
                            }
                            if (line.Contains("Artist:"))
                            {
                                artist = line.Replace("Artist:", "");
                            }
                            if (line.Contains("Version:"))
                            {
                                version = line.Replace("Version:", "");
                                newVersion = String.Format("{1}{3}{0}{4}{2}{5}", title, artist, version, " - ", " [", "] ");
                            }
                            if (line.Contains("AudioFilename:"))
                            {
                                audioFileName = line.Replace("AudioFilename: ", "");
                            }
                            if (line.Contains("0,0,\"") && line.Contains("\",0,0"))
                            {
                                backgroundFileName = line.Replace("0,0,\"", "").Replace("\",0,0", "");
                            }

                            if (backgroundFileName != null)
                            {
                                break;
                                
                            }
                        }
                        List<string> osuFile = new List<string>() { title, artist, version, newVersion, audioFileName, backgroundFileName };
                        mainDict.Add(path, osuFile);
                        backgroundFileName = null;
                    }

                }
            }
            //有关判断是否一首歌是否有不同音频和背景的逻辑
            foreach (var path in mainDict.Keys)
            {
                foreach (var path_2 in mainDict.Keys)
                {
                    string Optional = null;
                    if (mainDict[path][0] == mainDict[path_2][0] && mainDict[path][1] == mainDict[path_2][1])
                    {
                        if (mainDict[path][5] != mainDict[path_2][5] && mainDict[path][4] != mainDict[path_2][4])
                        {
                            if (opt.ContainsKey(path) == false)
                            {
                                Optional = "_" + mainDict[path][2];
                                opt.Add(path, new List<string> { "all", Optional });
                            }
                        }
                        else if (mainDict[path][4] != mainDict[path_2][4])
                        {
                            if (opt.ContainsKey(path) == false)
                            {
                                Optional = "_" + mainDict[path][2];
                                opt.Add(path, new List<string> { "audio", Optional });
                            }
                        }
                        else if (mainDict[path][5] != mainDict[path_2][5])
                        {
                            if (opt.ContainsKey(path) == false)
                            {
                                Optional = "_" + mainDict[path][2];
                                opt.Add(path, new List<string> { "bg", Optional });
                            }
                        }   
                    }
                }

            }
        }
    }
}
