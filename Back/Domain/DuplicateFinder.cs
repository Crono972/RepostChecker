using log4net;
using RepostChecker.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RepostChecker.Domain
{
    public class DuplicateFinder
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(DuplicateFinder));

        private IList<FacebookPost> _posts;
        private Context _context;
        private string _savePath;

        public DuplicateFinder(Context context, IList<FacebookPost> posts, string miniatureSavePath)
        {
            _posts = posts;
            _context = context;
            _savePath = miniatureSavePath;
        }

        public FacebookPost FindRepost(string postId)
        {
            string postFullid = string.Format("{0}_{1}", _context.GroupId, postId);

            var potentialRepost = GetPost(postFullid);

            if (potentialRepost == null)
            {
                logger.Error(string.Format("Post {0} doesn't exist", postId));
                throw new ArgumentException("Post doesn't exist");
            }

            var linkRepost = FindLinkRepost(potentialRepost);
            if (linkRepost != null)
            {
                logger.InfoFormat("Found link repost {0} by {1}", linkRepost.Id, linkRepost.Author);
                return linkRepost;
            }

            return FindByPicture(potentialRepost);
        }

        private FacebookPost GetPost(string postId)
        {
            return _posts.FirstOrDefault(s => s.Id == postId);
        }

        public FacebookPost FindLinkRepost(FacebookPost potentialRepost)
        {
            var otherPosts = _posts.Where(s => s.Id != potentialRepost.Id);
            if (string.IsNullOrEmpty(potentialRepost.Link))
            {
                return null;
            }

            return otherPosts.FirstOrDefault(s => s.Link == potentialRepost.Link);
        }

        public FacebookPost FindByPicture(FacebookPost potentialRepost)
        {
            if (string.IsNullOrEmpty(potentialRepost.PictureUrl))
            {
                logger.InfoFormat("Can't verify if {0} is a repost", potentialRepost.Id);
                return null;
            }

            var otherPosts = _posts.Where(s => s.Id != potentialRepost.Id);
            int downloadCounter = 1;
            Bitmap postToLookBitmap = Helper.GetImage(potentialRepost, _savePath);
            var listPicture = otherPosts.Where(s => s.PictureUrl != string.Empty).ToList();
            int total = listPicture.Count;
            foreach (var post in otherPosts)
            {
                var CompareBitMap = Helper.GetImage(post, _savePath);
                if (Helper.Compare(postToLookBitmap, CompareBitMap, 5))
                {
                    logger.InfoFormat("Found repost {0} by {1}", post.Id, post.Author);
                    return post;
                }
                logger.InfoFormat("Currently at {0}", (float)((float)downloadCounter / (float)total));
                downloadCounter++;
            }
            return null;
        }
    }
}
