using System.Collections.Generic;
using Facebook;
using RepostChecker.Model;

namespace RepostChecker.Infra
{
    public interface IFacebookWrapper
    {
        FacebookClient Client { get; }

        List<FacebookPost> GetPost();
        void PostComment(string postId, string message);
    }
}