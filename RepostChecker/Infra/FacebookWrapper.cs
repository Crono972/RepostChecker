using Facebook;
using log4net;
using Newtonsoft.Json;
using RepostChecker.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepostChecker.Infra
{
    public class FacebookWrapper : IFacebookWrapper
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(FacebookWrapper));
        private readonly FacebookClient _client;
        private Context _ctx;

        public FacebookClient Client
        {
            get { return _client; }
        }

        public FacebookWrapper(Context context)
        {
            _client = new FacebookClient(context.ApiToken);
            _ctx = context;
        }

        public List<FacebookPost> GetPost()
        {
            try
            {
                logger.InfoFormat("Starting the fetching of post {0}", _ctx.GroupId);
                var posts = new List<FacebookPost>();

                //On récupère les 250 premiers posts du groupes.
                var data = _client.Get(string.Format("{0}/{1}?limit=250", _ctx.GroupId, "feed"));
                var formattedData = JsonConvert.DeserializeObject<FacebookGroupFeed>(data.ToString());

                //On récupère tous l'historique du groupe.
                while (formattedData.Posts.Any())
                {
                    posts.AddRange(formattedData.Posts);
                    var temp = _client.Get(formattedData.paging.Next);
                    formattedData = JsonConvert.DeserializeObject<FacebookGroupFeed>(temp.ToString());
                    logger.InfoFormat("Now at {0}", posts.Count);
                }
                logger.InfoFormat("Fetched {0} post", posts.Count);
                return posts;
            }
            catch (Exception e)
            {
                logger.Error("Couldn't retrieve post", e);
                return null;
            }
        }

        public void PostComment(string postId, string message)
        {
            try
            {
                var commentDictionary = new Dictionary<string, object>
                                       {
                                           {"id", postId},
                                           {"from", _ctx.ApiToken},
                                           {"message", message}
                                       };
                _client.Post("/comments", commentDictionary);
            }
            catch (Exception e)
            {
                logger.Error("Couldn't post comment", e);
            }
        }
    }
}

