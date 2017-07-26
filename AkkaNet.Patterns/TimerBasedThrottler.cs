//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="TimerBasedThrottler.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-24, 6:09 AM
// Last changed by: profesor79, 2017-07-25, 10:50 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace AkkaNet.Patterns
{
  using System;

  using Akka.Actor;

  /// <summary>The testing.</summary>
  public class TimerBasedThrottler : ReceiveActor, IWithUnboundedStash
  {
    private readonly uint _messagesPerTimeUnit;

    private readonly uint _timeFrameInMiliseconds;

    private ICancelable _cancelTimer;

    private uint _counter;

    private bool _readyToCancelTimer;

    /// <summary>Initializes a new instance of the <see cref="TimerBasedThrottler"/> class.</summary>
    /// <param name="messagesPerTimeUnit">The messages per time unit.</param>
    /// <param name="timeFrameInMilliseconds">The time frame in miniseconds.</param>
    public TimerBasedThrottler(uint messagesPerTimeUnit, uint timeFrameInMilliseconds)
    {
      _messagesPerTimeUnit = messagesPerTimeUnit;
      _timeFrameInMiliseconds = timeFrameInMilliseconds;

      // as per scala implementation we start from idle pattern
      Receive<MessageToBeThrottled<string>>(
        message =>
          {
            // create timer 
            CreateTimer();

            // reset counter
            _counter = 0;

            SendMessageAndIncreaseCounter(message);

            if (_counter != _messagesPerTimeUnit)
            {
              Become(Throttling);
            }
            else
            {
              Become(Iddling);
            }
          });

      Receive<MessageTimer>(
        t =>
          {
            // stop timer to free system resources
            _cancelTimer?.Cancel();
          });
    }

    /// <summary>Gets or sets the stash.</summary>
    public IStash Stash { get; set; }

    private void CreateTimer()
    {
      _cancelTimer?.Cancel(); // cancel if exists
      _cancelTimer = new Cancelable(Context.System.Scheduler);
      Context.System.Scheduler.ScheduleTellRepeatedly(
        TimeSpan.FromMilliseconds(_timeFrameInMiliseconds),
        TimeSpan.FromMilliseconds(_timeFrameInMiliseconds),
        Self,
        new MessageTimer(),
        Self,
        _cancelTimer);
    }

    private void Iddling()
    {
      Receive<MessageToBeThrottled<string>>(message => { Stash.Stash(); });

      Receive<MessageTimer>(
        m =>
          {
            _counter = 0;
            Become(Throttling);
            Stash.Unstash();
          });
    }

    private void SendMessageAndIncreaseCounter(MessageToBeThrottled<string> message)
    {
      // send message
      message.Target.Tell(message.Content);
      _counter++;
    }

    private void Throttling()
    {
      Receive<MessageToBeThrottled<string>>(
        message =>
          {
            _readyToCancelTimer = false;
            SendMessageAndIncreaseCounter(message);

            if (_counter == _messagesPerTimeUnit)
            {
              Become(Iddling);
            }
            else
            {
              // retrieve message from stash if any
              Stash.Unstash();
            }
          });

      Receive<MessageTimer>(
        m =>
          {
            _counter = 0;

            if (_readyToCancelTimer)
            {
              // start with constructor receive 
              UnbecomeStacked();
            }
            else
            {
              _readyToCancelTimer = true;
            }
          });
    }

    public class MessageTimer
    {
    }

    public class MessageToBeThrottled<TContent>
    {
      public MessageToBeThrottled(IActorRef target, TContent content)
      {
        Target = target;
        Content = content;
      }

      public TContent Content { get; }

      public IActorRef Target { get; }
    }
  }
}
