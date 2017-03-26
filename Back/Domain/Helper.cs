using log4net;
using RepostChecker.Model;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using XnaFan.ImageComparison;

namespace RepostChecker
{
    public static class Helper
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(Helper));

        //Telecharge une image et la monte en mémoire depuis un post fb
        public static Bitmap GetImage(FacebookPost post, string savePath)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    if (string.IsNullOrEmpty(post.PictureUrl))
                    {
                        return null;
                    }
                    var fileName = string.Format(savePath + Path.DirectorySeparatorChar + "{0}.jpg", post.Id);
                    if (!File.Exists(fileName))
                    {
                        wc.DownloadFile(post.PictureUrl, fileName);
                    }
                    var image = Image.FromFile(fileName);
                    return (Bitmap)image;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Got an error while downloading post {0} : Title {1}", post.Id, post.Title), e);
                return null;
            }

        }

        //Compare deux images
        public static bool Compare(Bitmap firstImage, Bitmap secondImage, float seuilTolerance)
        {
            try
            {
                if (firstImage == null || secondImage == null)
                {
                    return false;
                }

                var difference = firstImage.PercentageDifference(secondImage, 3) * 100;
                return difference < seuilTolerance;
            }
            catch (Exception e)
            {
                Logger.Error("Got an error while comparing image", e);
                return false;
            }
        }

        public static string GetSavePath(string FolderName = "Save")
        {
            try
            {
                string codebase = Assembly.GetExecutingAssembly().Location;
                var exeFolder = new DirectoryInfo(Path.GetDirectoryName(codebase));
                var savePath = Path.Combine(exeFolder.FullName, FolderName);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                return savePath;
            }
            catch(Exception e)
            {
                Logger.Error("Got an error while creating folder to save miniature", e);
                throw;
            }
        }
    }
}
