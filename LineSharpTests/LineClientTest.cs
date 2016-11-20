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
        public void ParseTest()
        {
            var eventStr = ReadResource("events.json");
            var client = new LineClient("xx", "xx", "xx");
            var events = client.ParseEvent(eventStr);
        }
    }
}
