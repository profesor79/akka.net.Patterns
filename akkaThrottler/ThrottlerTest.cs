//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="ThrottlerTest.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-25, 11:47 PM
// Last changed by: profesor79, 2017-07-26, 12:15 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace akkaThrottler
{
  using System;

  using Akka.Actor;
  using Akka.TestKit;
  using Akka.TestKit.Xunit;

  using NUnit.Framework;

  public class ThrottlerTest : TestKit
  {
    private IActorRef _sut;

    private TestProbe _testProbe;

    [Test]
    public void Send6MessegasInLessThanSecond()
    {
      // arrange
      var message = new Wrapper<string>("test", _testProbe);

      // act
      for (var i = 0; i < 20; i++)
      {
        _sut.Tell(message);
      }

      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectNoMsg(TimeSpan.FromSeconds(1));
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectMsg("test");
      _testProbe.ExpectNoMsg(TimeSpan.FromSeconds(1));
      _testProbe.ExpectMsg("test");
    }

    [SetUp]
    public void Setup()
    {
      _sut = Sys.ActorOf(Props.Create(() => new ThrottlerActor(TimeSpan.FromSeconds(2), 5)));
      _testProbe = CreateTestProbe();
    }
  }
}
