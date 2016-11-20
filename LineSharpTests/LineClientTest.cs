using System.IO;
using System.Reflection;
using LineSharp;
using NUnit.Framework;

namespace LineSharpTests
{
    internal class LineClientTest
    {
        private string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"LineSharpTests.{resourceName}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void ValidateSignatureTest()
        {
            var eventStr = ReadResource("events.json");
            var client = new LineClient("channel_id", "channel_secret", "8d0cZjCcevNUFA8SlW4Ei8KroKKXmdJzZLMBjxrZuk1EpnTRxr2aek04EUGLuu6LU++u1WhTf6uiq03x4g350Q==");
            Assert.IsTrue(client.ValidateSignature(eventStr, "4wtQUcLa2kAlZBq34DTIRW0Tomzu2PLPo+N+OWEGO/k="));
        }

        [Test]
        public void ParseTest()
        {
            var eventStr = ReadResource("events.json");
            var client = new LineClient("xx", "xx", "xx");
            var events = client.ParseEvent(eventStr);
        }
    }
}
