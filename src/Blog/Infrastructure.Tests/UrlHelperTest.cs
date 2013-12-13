using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.Helpers;
namespace Infrastructure.Tests
{
    [TestClass]
    public class UrlHelperTest
    {
        [TestMethod]
        public void Create_url_slug()
        {
            string nonFriendlyUrl= "sasa/ &sasa@_";
            var frienlyUrl = URLHelper.ToFriendlyUrl(nonFriendlyUrl);
            Assert.IsNotNull(frienlyUrl);
            Assert.IsFalse(frienlyUrl.Contains(" "));
            Assert.IsFalse(frienlyUrl.Contains("/"));
            Assert.IsFalse(frienlyUrl.Contains("@"));
            Assert.IsFalse(frienlyUrl.Contains("&"));
            Assert.IsFalse(frienlyUrl.Contains("_"));
        }

        [TestMethod]
        public void Create_url_slug_with_id()
        {
            var nonFriendlyUrl = "sasa/ &sasa@";
            var r = new Random();
            var randomNumber = r.Next(15);
            var id = randomNumber.ToString();
            var frienlyUrl = URLHelper.ToFriendlyUrl(nonFriendlyUrl, id);
            Assert.IsNotNull(frienlyUrl);
            Assert.IsTrue(frienlyUrl.EndsWith("__"));
            Assert.IsTrue(frienlyUrl.Contains("__" + id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_url_slug_with_nonvalid_id()
        {
            var nonFriendlyUrl = "sasa/ &sasa@";
            var r = new Random();
            var randomNumber = r.Next(15);
            var id = randomNumber.ToString();
            id = id + "__ ";
            var frienlyUrl = URLHelper.ToFriendlyUrl(nonFriendlyUrl, id);
            Assert.IsNotNull(frienlyUrl);
            Assert.IsTrue(frienlyUrl.EndsWith("__"));
            Assert.IsTrue(frienlyUrl.Contains("__" + id));
        }

        [TestMethod]
        public void Extract_Id_From_Url_Slug()
        {
            var nonFriendlyUrl = "sasa/ &sasa@";
            var r = new Random();
            var randomNumber = r.Next(300);
            var id = randomNumber.ToString();

            var frienlyUrl = URLHelper.ToFriendlyUrl(nonFriendlyUrl, id);
            Assert.IsNotNull(frienlyUrl);
            var extractedId = URLHelper.GetIdFromString(frienlyUrl);
            Assert.AreEqual(id, extractedId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extract_Id_From_Url_Slug_From_Null()
        {
            string url = null;
            var extractedId = URLHelper.GetIdFromString(url);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extract_Id_From_Url_Slug_With_Bad_Url_1()
        {
            var nonFriendlyUrl = "sasa/ &sasa@_";
            var extractedId = URLHelper.GetIdFromString(nonFriendlyUrl);
        
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extract_Id_From_Url_Slug_With_Bad_Url_2()
        {
            var nonFriendlyUrl = "sasa/ &sasa@__";
            var extractedId = URLHelper.GetIdFromString(nonFriendlyUrl);

        }

    }
}
