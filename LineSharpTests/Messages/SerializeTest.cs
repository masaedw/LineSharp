using LineSharp.Messages;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LineSharpTests.Messages
{
    internal class SerializeTest
    {
        [Test]
        public void SerializeTemplateMessageTest()
        {
            var json = @"{
  ""type"": ""template"",
  ""altText"": ""this is a buttons template"",
  ""template"": {
      ""type"": ""buttons"",
      ""thumbnailImageUrl"": ""https://example.com/bot/images/image.jpg"",
      ""title"": ""Menu"",
      ""text"": ""Please select"",
      ""actions"": [
          {
            ""type"": ""postback"",
            ""label"": ""Buy"",
            ""data"": ""action=buy&itemid=123""
          },
          {
            ""type"": ""postback"",
            ""label"": ""Add to cart"",
            ""data"": ""action=add&itemid=123""
          },
          {
            ""type"": ""uri"",
            ""label"": ""View detail"",
            ""uri"": ""http://example.com/page/123""
          }
      ]
  }
}";

            var message = JsonConvert.DeserializeObject<SendMessageBase>(json);
            Assert.IsInstanceOf(typeof(TemplateMessage), message);

            var tm = (TemplateMessage)message;
            Assert.AreEqual("this is a buttons template", tm.AltText);
            Assert.IsNotNull(tm.Template);
            Assert.IsInstanceOf(typeof(ButtonsTemplate), tm.Template);

            var bt = (ButtonsTemplate)tm.Template;
            Assert.AreEqual("https://example.com/bot/images/image.jpg", bt.ThumbnailImageUrl);
            Assert.IsNotNull(bt.Actions);
        }
    }
}
