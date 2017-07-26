//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Class1.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-21, 11:52 PM
// Last changed by: profesor79, 2017-07-25, 12:31 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Akka.Net.Patterns.Tests
{
  using System;

  using Akka.Actor;
  using Akka.TestKit;
  using Akka.TestKit.Xunit2;

  using AkkaNet.Patterns;

  using NUnit.Framework;

  public class Class1 : TestKit
  {
    private IActorRef _sut;

    private TestProbe _testProbe;

    [Test]
    public void CreateActorAndSendMessage()
    {
      // arrange
      _sut = Sys.ActorOf(Props.Create(() => new TimerBasedThrottler(2, 6000)));
      var message = new TimerBasedThrottler.MessageToBeThrottled<string>(_testProbe, "aaaa");

      // act
      _sut.Tell(message);
      message = new TimerBasedThrottler.MessageToBeThrottled<string>(_testProbe, "bbbb");
      _sut.Tell(message);
      message = new TimerBasedThrottler.MessageToBeThrottled<string>(_testProbe, "ccc");
      _sut.Tell(message);

      // assert
      _testProbe.ExpectMsg("aaaa", TimeSpan.FromMilliseconds(200));
      _testProbe.ExpectMsg("bbbb", TimeSpan.FromMilliseconds(200));
      NoMessageSent(2800);
      _testProbe.ExpectMsg("ccc", TimeSpan.FromMilliseconds(500));
    }

    /// <summary>The no message sent.</summary>
    /// <param name="milliSeconds">The i.</param>
    public void NoMessageSent(int milliSeconds = 100)
    {
      ExpectNoMsg(TimeSpan.FromMilliseconds(milliSeconds));
      _testProbe.ExpectNoMsg(TimeSpan.FromMilliseconds(milliSeconds));
    }

    /// <summary>The setup.</summary>
    [SetUp]
    public void Setup() { _testProbe = CreateTestProbe("testProbe"); }

    /// <summary>The tear down.</summary>
    [TearDown]
    public void TearDown() { _testProbe.Tell(PoisonPill.Instance); }
  }
}
