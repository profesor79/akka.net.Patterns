//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="ThrottlerActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-25, 11:42 PM
// Last changed by: profesor79, 2017-07-26, 12:38 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace akkaThrottler
{
  using System;

  using Akka.Actor;

  /// <summary>The throttler actor.</summary>
  public partial class ThrottlerActor : ReceiveActor, IWithUnboundedStash
  {
    /// <summary>The _interval.</summary>
    private readonly TimeSpan _interval;

    /// <summary>The _number of messages.</summary>
    private readonly uint _numberOfMessages;

    /// <summary>The _cancel timer.</summary>
    private ICancelable _cancelTimer;

    /// <summary>The _counter.</summary>
    private uint _counter;

    /// <summary>Initializes a new instance of the <see cref="ThrottlerActor"/> class.</summary>
    /// <param name="interval">The interval.</param>
    /// <param name="numberOfMessages">The number of messages.</param>
    public ThrottlerActor(TimeSpan interval, uint numberOfMessages)
    {
      _interval = interval;
      _numberOfMessages = numberOfMessages;
      Receive<Wrapper<string>>(
        message =>
          {
            InitTimerAndCounter();

            SendMessage(message);

            SetNextState();
          });

      Receive<MessageTimer>(t => { _cancelTimer?.Cancel(); });
    }

    public IStash Stash { get; set; }

    private void Iddling()
    {
      Receive<Wrapper<string>>(message => { Stash.Stash(); });

      Receive<MessageTimer>(
        t =>
          {
            _counter = 0;
            Become(Throttling);
            Stash.Unstash();
          });
    }

    private void InitTimerAndCounter()
    {
      _counter = 0;
      _cancelTimer?.Cancel();
      _cancelTimer = new Cancelable(Context.System.Scheduler);
      Context.System.Scheduler.ScheduleTellRepeatedly(_interval, _interval, Self, new MessageTimer(), Self, _cancelTimer);
    }

    private void SendMessage(Wrapper<string> message)
    {
      // send message
      message.DestinationActor.Tell(message.Message);
      _counter++;
    }

    private void SetNextState()
    {
      if (_numberOfMessages != _counter)
      {
        Become(Throttling);
        Stash.Unstash();
      }
      else
      {
        Become(Iddling);
      }
    }

    private void Throttling()
    {
      Receive<Wrapper<string>>(
        message =>
          {
            SendMessage(message);

            SetNextState();
          });

      Receive<MessageTimer>(t => { UnbecomeStacked(); });
    }
  }
}
