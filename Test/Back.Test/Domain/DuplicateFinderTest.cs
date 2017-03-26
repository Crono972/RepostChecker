using NUnit.Framework;
using RepostChecker.Domain;
using RepostChecker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Back.Test.Domain
{
    [TestFixture]
    public class DuplicateFinderTest
    {
        [Test]
        public void Should_Find_a_repost_link()
        {
            var repost = TestHelper.GenerateFakePost(1, "post1", "http://www.google.fr");
            var fakeList = new List<FacebookPost>
            {
               repost,
               TestHelper.GenerateFakePost(2,"post2", "http://www.google.fr"),
               TestHelper.GenerateFakePost(3,"post3", "http://www.google.fr"),
            };
            var context = new Context { ApiToken = "", GroupId = Constants.Group.TrolololTest };
            var dupFinder = new DuplicateFinder(context, fakeList, string.Empty);

            FacebookPost originalPost = dupFinder.FindLinkRepost(repost);
            Assert.IsNotNull(originalPost);

        }
    }
}
