//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="EvenlyThrottlerActorTest.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-26, 12:36 AM
// Last changed by: profesor79, 2017-07-26, 1:03 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace akkaThrottler
{
  using System;

  using Akka.Actor;
  using Akka.TestKit;
  using Akka.TestKit.Xunit;

  using NUnit.Framework;

  public class EvenlyThrottlerActorTest : TestKit
  {
    private IActorRef _sut;

    private TestProbe _testprobe;

    [Test]
    public void Send6Messages()
    {
      // arrange
      var message = new Wrapper<string>("hello", _testprobe);

      // act
      for (var i = 0; i < 20; i++)
      {
        _sut.Tell(message);
      }

      // assert
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();

      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
      _testprobe.ExpectMsg("hello");
      _testprobe.ExpectNoMsg();
    }

    [SetUp]
    public void Setup()
    {
      _sut = Sys.ActorOf(Props.Create(() => new EvenlyThrottlerActor(TimeSpan.FromSeconds(18), 5)));
      _testprobe = CreateTestProbe();
    }
  }
}
