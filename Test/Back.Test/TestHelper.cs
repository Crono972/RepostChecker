using RepostChecker.Domain;
using RepostChecker.Model;

namespace Back.Test
{
    public static class TestHelper
    {
        public static FacebookPost GenerateFakePost(long id, string title = "", string link = "", string pictureUrl = "")
        {
            return new FacebookPost
            {
                Id = string.Format("{0}_{1}", Constants.Group.TrolololTest, id),
                Author = new User { Id = "1", Name = "TestUser" },
                Title = title,
                Link = link,
                PictureUrl = pictureUrl
            };
        }
    }
}
