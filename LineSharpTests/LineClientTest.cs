using System.IO;
using System.Linq;
using System.Reflection;
using LineSharp;
using LIneSharp.Messages;
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
            var client = new LineClient("channel_id", "QUNDRVNTX1RPS0VO", "channel_access_token");
            Assert.IsTrue(client.ValidateSignature(eventStr, "8kcX2m/vS6vlIkZAEZ5usNMl9BRleaB1alXJOHkW4OU="));
        }

        [Test]
        public void ParseTest()
        {
            var eventStr = ReadResource("events.json");
            var client = new LineClient("xx", "xx", "xx");
            var events = client.ParseEvent(eventStr).ToArray();

            Assert.IsInstanceOf<MessageEvent>(events[0]);
            var event0 = (MessageEvent)events[0];
            Assert.IsInstanceOf<UserSoruce>(event0.Source);
            Assert.IsInstanceOf<TextMessage>(event0.Message);

            Assert.IsInstanceOf<MessageEvent>(events[1]);
            var event1 = (MessageEvent)events[1];
            Assert.IsInstanceOf<UserSoruce>(event1.Source);
            Assert.IsInstanceOf<ImageMessage>(event1.Message);

            Assert.IsInstanceOf<MessageEvent>(events[2]);
            var event2 = (MessageEvent)events[2];
            Assert.IsInstanceOf<UserSoruce>(event2.Source);
            Assert.IsInstanceOf<VideoMessage>(event2.Message);

            Assert.IsInstanceOf<MessageEvent>(events[3]);
            var event3 = (MessageEvent)events[3];
            Assert.IsInstanceOf<UserSoruce>(event3.Source);
            Assert.IsInstanceOf<AudioMessage>(event3.Message);

            Assert.IsInstanceOf<MessageEvent>(events[4]);
            var event4 = (MessageEvent)events[4];
            Assert.IsInstanceOf<UserSoruce>(event4.Source);
            Assert.IsInstanceOf<LocationMessage>(event4.Message);

            Assert.IsInstanceOf<MessageEvent>(events[5]);
            var event5 = (MessageEvent)events[5];
            Assert.IsInstanceOf<UserSoruce>(event5.Source);
            Assert.IsInstanceOf<StickerMessage>(event5.Message);

            Assert.IsInstanceOf<FollowEvent>(events[6]);
            var event6 = (FollowEvent)events[6];
            Assert.IsInstanceOf<UserSoruce>(event6.Source);

            Assert.IsInstanceOf<UnfollowEvent>(events[7]);
            var event7 = (UnfollowEvent)events[7];
            Assert.IsInstanceOf<UserSoruce>(event7.Source);

            Assert.IsInstanceOf<JoinEvent>(events[8]);
            var event8 = (JoinEvent)events[8];
            Assert.IsInstanceOf<GroupSoruce>(event8.Source);

            Assert.IsInstanceOf<LeaveEvent>(events[9]);
            var event9 = (LeaveEvent)events[9];
            Assert.IsInstanceOf<GroupSoruce>(event9.Source);

            Assert.IsInstanceOf<PostbackEvent>(events[10]);
            var event10 = (PostbackEvent)events[10];
            Assert.IsInstanceOf<UserSoruce>(event10.Source);
            Assert.AreEqual("action=buyItem&itemId=123123&color=red", event10.Postback.Data);

            Assert.IsInstanceOf<BeaconEvent>(events[11]);
            var event11 = (BeaconEvent)events[11];
            Assert.IsInstanceOf<UserSoruce>(event11.Source);
            Assert.AreEqual("d41d8cd98f", event11.Beacon.Hwid);
        }
    }
}
