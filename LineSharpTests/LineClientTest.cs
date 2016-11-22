using System.IO;
using System.Linq;
using System.Reflection;
using LineSharp;
using LineSharp.Messages;
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
            var e = "{\"events\":[{\"replyToken\":\"00000000000000000000000000000000\",\"type\":\"message\",\"timestamp\":1451617200000,\"source\":{\"type\":\"user\",\"userId\":\"Udeadbeefdeadbeefdeadbeefdeadbeef\"},\"message\":{\"id\":\"100001\",\"type\":\"text\",\"text\":\"Hello,world\"}},{\"replyToken\":\"ffffffffffffffffffffffffffffffff\",\"type\":\"message\",\"timestamp\":1451617210000,\"source\":{\"type\":\"user\",\"userId\":\"Udeadbeefdeadbeefdeadbeefdeadbeef\"},\"message\":{\"id\":\"100002\",\"type\":\"sticker\",\"packageId\":\"1\",\"stickerId\":\"1\"}}]}";
            var c = new LineClient("channel_id", "9a6f07fc8f2d3723fd1ac2ab3411e38e", "channel_access_tokn");
            Assert.IsTrue(c.ValidateSignature(e, "cRUG4B7ACUM0Z1UefRxcXynmPEHmzcw7RkXN6Z/HuGs="));
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
            Assert.IsInstanceOf<TextEventMessage>(event0.Message);

            Assert.IsInstanceOf<MessageEvent>(events[1]);
            var event1 = (MessageEvent)events[1];
            Assert.IsInstanceOf<UserSoruce>(event1.Source);
            Assert.IsInstanceOf<ImageEventMessage>(event1.Message);

            Assert.IsInstanceOf<MessageEvent>(events[2]);
            var event2 = (MessageEvent)events[2];
            Assert.IsInstanceOf<UserSoruce>(event2.Source);
            Assert.IsInstanceOf<VideoEventMessage>(event2.Message);

            Assert.IsInstanceOf<MessageEvent>(events[3]);
            var event3 = (MessageEvent)events[3];
            Assert.IsInstanceOf<UserSoruce>(event3.Source);
            Assert.IsInstanceOf<AudioEventMessage>(event3.Message);

            Assert.IsInstanceOf<MessageEvent>(events[4]);
            var event4 = (MessageEvent)events[4];
            Assert.IsInstanceOf<UserSoruce>(event4.Source);
            Assert.IsInstanceOf<LocationEventMessage>(event4.Message);

            Assert.IsInstanceOf<MessageEvent>(events[5]);
            var event5 = (MessageEvent)events[5];
            Assert.IsInstanceOf<UserSoruce>(event5.Source);
            Assert.IsInstanceOf<StickerEventMessage>(event5.Message);

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
