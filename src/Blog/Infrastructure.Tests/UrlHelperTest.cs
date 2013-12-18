using Infrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Tests
{
    [TestClass]
    public class UrlHelperTest
    {
        [TestMethod]
        public void Create_url_slug()
        {
            // Arrange
            string nonFriendlyUrl = "sasa/ &sasa@_";

            // Act
            var frienlyUrl = URLHelper.ToFriendlyUrl(nonFriendlyUrl);

            // Assert
            Assert.IsNotNull(frienlyUrl);
            Assert.IsFalse(frienlyUrl.Contains(" "));
            Assert.IsFalse(frienlyUrl.Contains("/"));
            Assert.IsFalse(frienlyUrl.Contains("@"));
            Assert.IsFalse(frienlyUrl.Contains("&"));
            Assert.IsFalse(frienlyUrl.Contains("_"));
        }
    }
}