using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Styx.Helpers;
using Styx.Common;

namespace BloodyMessNG
{
    public static class Updater
    {
        private const string URL = "http://bloodymess.googlecode.com/svn/trunk/BloodyMess/";
        private const string ChangeLogUrl = "http://code.google.com/p/bloodymess/source/detail?r=";
        private static readonly Regex _linkPattern = new Regex(@"<li><a href="".+"">(?<ln>.+(?:..))</a></li>",
                                                               RegexOptions.CultureInvariant);
        private static readonly Regex _changelogPattern =
            new Regex(
                "<h4 style=\"margin-top:0\">Log message</h4>\r?\n?<pre class=\"wrap\" style=\"margin-left:1em\">(?<log>.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?)</pre>",
                RegexOptions.CultureInvariant);
        private static BloodyMessNGForm BloodyMessNGConfig;

        public static void CheckForUpdate()
        {
            try
            {
                Logging.Write(LogLevel.Normal, Colors.White, "Checking for new version");
                int remoteRev = GetRevision();
                if(BloodyMessNGConfig == null)
                    BloodyMessNGConfig = new BloodyMessNGForm();
                if (BloodyMessNGConfig.Settings.RevisionNumber != remoteRev)
                {
                    Logging.Write(LogLevel.Normal, Colors.White, "A new version was found");

                    Logging.Write(LogLevel.Normal, Colors.White, "Downloading Update");
                    DownloadFilesFromSvn(new WebClient(), URL);
                    Logging.Write(LogLevel.Normal, Colors.White, "Download complete");
                    Logging.Write(LogLevel.Normal, Colors.White, "Removing old files");
                    clearNonNewFiles(BloodyMessNG.DeathKnight.baseFolder);
                    Logging.Write(LogLevel.Normal, Colors.White, "Deleting complete");
                    Logging.Write(LogLevel.Normal, Colors.White, "Renaming .new files to usable");
                    renameFiles(BloodyMessNG.DeathKnight.baseFolder);
                    Logging.Write(LogLevel.Normal, Colors.White, ".new files renamed");
                    Logging.Write(LogLevel.Normal, Colors.White, "Deleting leftover .new files");
                    clearNewFiles(BloodyMessNG.DeathKnight.baseFolder);
                    Logging.Write(LogLevel.Normal, Colors.White, "Done deleting");
                    BloodyMessNGConfig.Settings.RevisionNumber = remoteRev;
                    BloodyMessNGConfig.Settings.Save();
                    Logging.Write(LogLevel.Normal, Colors.White, "************* Change Log ****************");
                    Logging.Write(LogLevel.Normal, Colors.White, GetChangeLog(remoteRev));
                    Logging.Write(LogLevel.Normal, Colors.White, "*****************************************");
                    Logging.Write(LogLevel.Normal, Colors.White, "A new version of BloodyMessNG was installed. Please restart Honorbuddy");
                }
                else
                {
                    Logging.Write(LogLevel.Normal, Colors.White, "No updates found");
                }
            }
            catch (Exception ex)
            {
                 Logging.Write(LogLevel.Normal, Colors.White, ex.ToString());
            }
        }
        private static void clearNonNewFiles(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            foreach (FileInfo fi in dir.GetFiles())
            {
                if (fi.Name.IndexOf(".new") < 0 && fi.Name.IndexOf(".xml") < 0)
                {
                    Logging.Write(LogLevel.Normal, Colors.White, "Deleting " + fi.Name);
                    fi.Delete();
                }
            }
            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                clearNonNewFiles(di.FullName);
            }
        }
        private static void clearNewFiles(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            foreach (FileInfo fi in dir.GetFiles())
            {
                if (fi.Name.IndexOf(".new") > -1)
                {
                    Logging.Write(LogLevel.Normal, Colors.White, "Deleting " + fi.Name);
                    fi.Delete();
                }
            }
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearNewFiles(di.FullName);
            }
        }
        private static void renameFiles(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            foreach (FileInfo fi in dir.GetFiles())
            {
                if (fi.Name.IndexOf(".new") > -1)
                {
                    Logging.Write(LogLevel.Normal, Colors.White, "Renaming " + fi.Name + " to " + FolderName + fi.Name.Substring(0, fi.Name.IndexOf(".new")));
                    fi.MoveTo(FolderName + fi.Name.Substring(0, fi.Name.IndexOf(".new")));
                }
            }
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                renameFiles(di.FullName);
            }
        }
        private static int GetRevision()
        {
            var client = new WebClient();
            string html = client.DownloadString(URL);
            var pattern = new Regex(@" - Revision (?<rev>\d+):", RegexOptions.CultureInvariant);
            Match match = pattern.Match(html);
            if (match.Success && match.Groups["rev"].Success)
                return int.Parse(match.Groups["rev"].Value);
            throw new Exception("Unable to retreive revision");
        }
        private static void DownloadFilesFromSvn(WebClient client, string url)
        {
            string html = client.DownloadString(url);
            MatchCollection results = _linkPattern.Matches(html);

            IEnumerable<Match> matches = from match in results.OfType<Match>()
                                         where match.Success && match.Groups["ln"].Success
                                         select match;
            foreach (Match match in matches)
            {
                string file = RemoveXmlEscapes(match.Groups["ln"].Value);
                string newUrl = url + file;
                if (newUrl[newUrl.Length - 1] == '/') // it's a directory...
                {
                    DownloadFilesFromSvn(client, newUrl);
                }
                else // its a file.
                {
                    string filePath, dirPath;
                    string relativePath = url.Substring(URL.Length);
                    dirPath = Path.Combine(BloodyMessNG.DeathKnight.baseFolder,relativePath);
                    filePath = Path.Combine(dirPath, file + ".new");
                    Logging.Write(LogLevel.Normal, Colors.White, "Downloading {0}", file);
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                    client.DownloadFile(newUrl, filePath);
                }
            }
        }
        private static string RemoveXmlEscapes(string xml)
        {
            return
                xml.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace(
                    "&apos;", "'");
        }
        private static string GetChangeLog(int revision)
        {
            var client = new WebClient();
            string html = client.DownloadString(ChangeLogUrl + revision);
            Match match = _changelogPattern.Match(html);
            if (match.Success && match.Groups["log"].Success)
                return RemoveXmlEscapes(match.Groups["log"].Value);
            return null;
        }
    }
}