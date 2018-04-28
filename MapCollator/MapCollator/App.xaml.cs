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


namespace MapCollator
{
    public class GlobalValue
    {
        public static string path;
    }
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public class Program
        {
            public static void Start(string path, string packName, string artists, string creator, string OD, string HP)
            {

                //创建新文件夹存放生成的新文件
                string folderName = Path.Combine(Path.Combine(path.Replace(Path.GetFileName(path), ""), String.Format("{0}{1}{2}", artists, " - ", packName)));
                Directory.CreateDirectory(folderName);

                if (path != String.Empty)
                {
                    List<string> allFileList = IO.GetFileList(path);
                    //循环读取每个osu文件以及修改
                    foreach (string item in allFileList)
                    {
                        if (item.Contains(".osu"))
                        {
                            List<string> datas = IO.ReadFile(item, packName, artists, creator, HP, OD, folderName);

                            //string audioFileFormat = Path.GetExtension(datas[3]);
                            //string backgroundFormat = Path.GetExtension(datas[4]);

                        }
                    }
                    //打包到osz
                    IO.PackToOsz(folderName);
                    //删除临时文件夹
                    Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName), true);
                    MessageBox.Show("Packaged successfully!");
                }
            }
        }

    }
    class IO
    {

        public static List<string> ReadFile(string path, string packName, string artists, string creator, string HP, string OD, string folderName)
        {
            string line = null, title = null, artist = null, version = null, audioFileName = null, backgroundFileName = null;
            string newFilePathTemp = Path.Combine(folderName, "tmp");
            string newVersion = null, newAudioFileName = null, newBgFileName = null;
            string pathWithOutFileName = path.Replace(Path.GetFileName(path), "");
            string audioFileFormat, backgroundFileFormat;
            //读取文件
            using (StreamReader f = new StreamReader(path))
            {
                //创建新文件
                using (StreamWriter fw = new StreamWriter(newFilePathTemp))
                {
                    while ((line = f.ReadLine()) != null)
                    {
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
                        if (line.Contains("Version:"))
                        {
                            version = line.Replace("Version:", "");
                            newVersion = String.Format("{1}{3}{0}{4}{2}{5}", title, artist, version, " - ", " [", "] ");
                            line = String.Format("{0}{1}", "Version:", newVersion);
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
            //过滤掉文件名中的非法字符
            string newFilePath = FilterIllegalChars(String.Format("{1}{3}{0}{4}{2}{5}{7}{6}{8}{9}", packName, artists, newVersion, " - ", " [", "] ", creator, "(", ")", ".osu"));

            using (StreamReader f = new StreamReader(newFilePathTemp))
            {
                //创建新文件
                using (StreamWriter fw = new StreamWriter(Path.Combine(folderName, newFilePath)))
                {
                    while ((line = f.ReadLine()) != null)
                    {
                        if (line.Contains("AudioFilename:"))
                        {
                            audioFileName = line.Replace("AudioFilename: ", "");
                            audioFileFormat = Path.GetExtension(audioFileName);
                            //新的音频文件名字
                            line = FilterIllegalChars_2(String.Format("{4}{0}{1}{2}{3}", artist, " - ", title, audioFileFormat, "AudioFilename: "));
                            newAudioFileName = String.Format("{0}{1}{2}{3}", artist, " - ", title, audioFileFormat);
                            newAudioFileName = FilterIllegalChars_2(newAudioFileName);
                        }
                        if (line.Contains("0,0,\"") && line.Contains("\",0,0"))
                        {
                            backgroundFileName = line.Replace("0,0,\"", "").Replace("\",0,0", "");
                            backgroundFileFormat = Path.GetExtension(backgroundFileName);
                            //新的bg文件名字
                            line = String.Format("{4}{0}{1}{2}{3}{5}", FilterIllegalChars_2(artist), " - ", FilterIllegalChars_2(title), backgroundFileFormat, "0,0,\"", "\",0,0");
                            newBgFileName = String.Format("{0}{1}{2}{3}", artist, " - ", title, backgroundFileFormat);
                            newBgFileName = FilterIllegalChars_2(newBgFileName);
                        }
                        //这里用来写入修改后的内容
                        fw.WriteLine(line);
                    }
                }
            }

            //复制所关联的音频和bg文件到新的文件夹并应用新的名字
            string oldAudioFilePath = Path.Combine(pathWithOutFileName, audioFileName);
            string oldBgFilePath = Path.Combine(pathWithOutFileName, backgroundFileName);
            string newAudioFilePath = Path.Combine(folderName, newAudioFileName);
            string newBgFilePath = Path.Combine(folderName, newBgFileName);
            try
            {
                File.Copy(oldAudioFilePath, newAudioFilePath);
                File.Copy(oldBgFilePath, newBgFilePath);
            }
            catch (FileNotFoundException e)
            {
            }
            catch (IOException)
            {

            }
            List<string> datas = new List<string>
            {
                title, artist, version, audioFileName, backgroundFileName, OD, HP
            };
            return datas;
        }
        //遍历文件夹
        public static List<string> GetFileList(string rootPath)
        {
            string[] pathList = Directory.GetDirectories(rootPath);
            string[] fileList;
            List<string> allFileList = new List<string>();
            foreach (string item in pathList)
            {
                fileList = Directory.GetFiles(item);
                foreach (string file in fileList)
                {
                    allFileList.Add(file);
                }
            }
            return allFileList;
        }
        public static string FilterIllegalChars(string path)
        {
            string str = Path.GetFileName(path);
            str = str.Replace("\\", "_");
            str = str.Replace("/", "_");
            str = str.Replace("*", "_");
            str = str.Replace("?", "_");
            str = str.Replace("\"", "_");
            str = str.Replace("<", "_");
            str = str.Replace(">", "_");
            str = str.Replace("|", "_");
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
            return str;

        }
        public static void PackToOsz(string rootPath)
        {
            ZipFile.CreateFromDirectory(rootPath, rootPath + ".osz");
        }
    }

}
