using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Net.Patterns.Tests
{
    using Akka.Actor;
    using Akka.TestKit;
    using Akka.TestKit.Xunit2;

    using NUnit.Framework;

    public class Class1 : TestKit
    {
        private TestProbe _testProbe;

        /// <summary>The no message sent.</summary>
        /// <param name="i">The i.</param>
        public void NoMessageSent(int i = 100)
        {
            ExpectNoMsg(TimeSpan.FromMilliseconds(i));
            _testProbe.ExpectNoMsg(TimeSpan.FromMilliseconds(i));
        }

        /// <summary>The setup.</summary>
        [SetUp]
        public void Setup()
        {
            _testProbe = CreateTestProbe("testProbe");
        }

        /// <summary>The tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            _testProbe.Tell(PoisonPill.Instance);

        }
    }
}
