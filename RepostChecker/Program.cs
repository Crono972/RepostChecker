using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XnaFan.ImageComparison;

namespace RepostChecker
{
    class Program
    {
        private static string _savePath;
        static int downloadCounter = 1;

        static void Main()
        {
            //Variable
            string ApiKey = "EAACEdEose0cBADKcGNWxp9cYqj0LsS63ZADO0CIm1kX2Y2MD3NZB4vh7CuhnnUoIo159fXeS5BnMYQtFvRMnYB5bhXYYiEs3Csje7Yk4d3M5ci7MTbZCwMm7MTxVagfZC0xK0ZAtmvwjiUBUkxZA0S9ErIZBg7mUq4xZCt9OwB8I3wZDZD";
            //string trolololId = "264695076893206"; // Trololol
            string trolololId = "1360768953987327"; // Trololol test
            string postDouteux = "1360799297317626"; // Post a tester

            //Repertoire de sauvegarde des miniatures
            string codebase = Assembly.GetExecutingAssembly().Location;
            var exeFolder = new DirectoryInfo(Path.GetDirectoryName(codebase));
            _savePath = Path.Combine(exeFolder.FullName, "Save");
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }

            //Connecteur Facebook
            var wrapper = new FacebookWrapper(ApiKey);

            //List ou on va tout stocker
            var posts = new List<FacebookPost>();

            //On récupère les 250 premiers posts du groupes.
            var data = wrapper.Client.Get(string.Format("{0}/{1}?limit=250", trolololId, "feed"));
            var formattedData = JsonConvert.DeserializeObject<FacebookGroupFeed>(data.ToString());

            //On récupère tous l'historique du groupe.
            while (formattedData.Posts.Any())
            {
                posts.AddRange(formattedData.Posts);
                var temp = wrapper.Client.Get(formattedData.paging.Next);
                formattedData = JsonConvert.DeserializeObject<FacebookGroupFeed>(temp.ToString());
                Console.WriteLine(string.Format("Now at {0}", posts.Count));
            }

            Console.WriteLine("######################################################################");
            Console.WriteLine(string.Format("Il y a {0} post sur le groupe {1}", posts.Count, trolololId));

            string postIdToFindDuplicate = string.Format("{0}_{1}", trolololId, postDouteux);
            //On verifie que le post douteux existe
            if (!posts.Any(s => s.Id == postIdToFindDuplicate))
            {
                Console.WriteLine("Post douteux non trouvé");
                Console.ReadKey();
                return;
            }

            //Message posté si le repost est prouvé
            //var message = new { message = string.Format("https://www.facebook.com/groups/{0}/permalink/{1} est un repost", trolololId, postDouteux) };

            //Le post douteux
            var postToLook = posts.Single(s => s.Id == postIdToFindDuplicate);

            //Les autres postes
            var otherPost = posts.Where(s => s.Id != postToLook.Id).ToList();

            FacebookPost original = null;
            bool found = false;

            //On cherche dans les liens
            if (postToLook.Link != string.Empty)
            {
                var listDesRepost = otherPost.Where(s => s.Link == postToLook.Link).ToList();
                if (listDesRepost.Any())
                {
                    original = listDesRepost.First();
                    found = true;
                }
            }

            //On compare les images
            if (postToLook.PictureUrl != string.Empty && !found)
            {
                Bitmap postToLookBitmap = Helper.GetImage(postToLook);
                var listPicture = otherPost.Where(s => s.PictureUrl != string.Empty).ToList();
                int total = listPicture.Count;
                foreach (var post in listPicture)
                {
                    var CompareBitMap = Helper.GetImage(post);
                    if (Helper.Compare(postToLookBitmap, CompareBitMap, 5))
                    {
                        //wrapper.Client.Post(string.Format("{0}/comments", post.Id), message);
                        found = true;
                        original = post;
                        break;
                    }
                    Console.WriteLine(string.Format("Currently at {0}", (float)((float)downloadCounter / (float)total)));
                    downloadCounter++;
                }
            }
            if (!found)
            {
                Console.WriteLine("nothing found");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Repost found");           
                var message = string.Format("https://www.facebook.com/groups/{0}/permalink/{1} est un repost", trolololId, postDouteux);
                var commentDicitonay = new Dictionary<string, object>
                                       {
                                           {"id", original.Id},
                                           {"from", ApiKey},
                                           {"message", message}
                                       };
                wrapper.Client.Post("/comments", commentDicitonay);
                //wrapper.Client.Post(string.Format("{0}/comments", id_post), message);
                    Console.WriteLine("Message left msg on fb");
                
                Console.ReadKey();
            }
        }

        public static class Helper
        {
            //Telecharge une image et la monte en mémoire depuis un post fb
            public static Bitmap GetImage(FacebookPost post)
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        if (string.IsNullOrEmpty(post.PictureUrl))
                        {
                            return null;
                        }
                        var fileName = string.Format(_savePath + Path.DirectorySeparatorChar + "{0}.jpg", post.Id);
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
                    //Do Nothing
                    return null;
                }

            }

            //Compare deux images
            public static bool Compare(Bitmap firstImage, Bitmap secondImage, float seuilTolerance)
            {
                if (firstImage == null || secondImage == null)
                {
                    return false;
                }

                var difference = firstImage.PercentageDifference(secondImage, 3) * 100;
                return difference < seuilTolerance;
            }
        }

        //Connecteur Fb
        public class FacebookWrapper
        {
            readonly FacebookClient _client;

            public FacebookClient Client
            {
                get { return _client; }
            }

            public FacebookWrapper(string AccessToken)
            {
                _client = new FacebookClient(AccessToken);
            }
        }
    }
}
