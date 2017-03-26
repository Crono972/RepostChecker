using log4net;
using log4net.Config;
using RepostChecker.Domain;
using RepostChecker.Infra;
using RepostChecker.Model;
using System;
using System.IO;
using System.Reflection;

namespace RepostChecker
{
    public class Program
    {
        private static ILog logger;

        static void Main()
        {
            InitLog();

            //Variable
            var context = new Context
            {
                ApiToken = "EAACEdEose0cBADKcGNWxp9cYqj0LsS63ZADO0CIm1kX2Y2MD3NZB4vh7CuhnnUoIo159fXeS5BnMYQtFvRMnYB5bhXYYiEs3Csje7Yk4d3M5ci7MTbZCwMm7MTxVagfZC0xK0ZAtmvwjiUBUkxZA0S9ErIZBg7mUq4xZCt9OwB8I3wZDZD",
                GroupId = "1360768953987327"
            };
            string postDouteux = "1360799297317626"; // Post a tester

            //Connecteur Facebook
            IFacebookWrapper wrapper = new FacebookWrapper(context);
            var posts = wrapper.GetPost();

            var duplicateFinder = new DuplicateFinder(context, posts, Helper.GetSavePath());
            FacebookPost original = duplicateFinder.FindRepost(postDouteux);

            if (original == null)
            {
                logger.Info("nothing found");
                Console.ReadKey();
            }
            else
            {
                logger.Info("Repost found");
                var message = string.Format("https://www.facebook.com/groups/{0}/permalink/{1} est un repost", context.GroupId, postDouteux);
                wrapper.PostComment(original.Id, message);
                logger.Info("Message left msg on fb");
                Console.ReadKey();
            }
        }

        public static void InitLog(string logName = "RepostChecker")
        {
            GlobalContext.Properties["LogName"] = logName;
            XmlConfigurator.Configure(new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Log4net.config")));
            logger = LogManager.GetLogger(typeof(Program));
        }
    }
}
