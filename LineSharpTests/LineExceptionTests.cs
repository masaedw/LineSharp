using LineSharp.Messages;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LineSharp.Tests
{
    public class LineExceptionTests
    {
        [Test]
        public void LineExceptionTest_Message()
        {
            var ex = new LineException("message");
            Assert.AreEqual("message", ex.Message);
        }

        [Test]
        public void LineExceptionTest_Message_ErrorResponse()
        {
            var json = @"
{
   ""message"":""The request body has 2 error(s)"",
   ""details"":[
      {
         ""message"":""May not be empty"",
         ""property"":""messages[0].text""
      },
      {
         ""message"":""Must be one of the following values: [text, image, video, audio, location, sticker, template, imagemap]"",
         ""property"":""messages[1].type""
      }
   ]
}";

            var res = JsonConvert.DeserializeObject<ErrorResponse>(json);

            var ex = new LineException("message", res);

            var expected = "message\nThe request body has 2 error(s)\n  messages[0].text: May not be empty\n  messages[1].type: Must be one of the following values: [text, image, video, audio, location, sticker, template, imagemap]";

            Assert.AreEqual(expected, ex.Message);
        }
    }
}