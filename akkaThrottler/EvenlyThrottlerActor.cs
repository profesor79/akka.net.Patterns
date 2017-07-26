//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="EvenlyThrottlerActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-26, 12:36 AM
// Last changed by: profesor79, 2017-07-26, 1:05 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace akkaThrottler
{
  using System;

  using Akka.Actor;

  public class EvenlyThrottlerActor : ReceiveActor, IWithUnboundedStash
  {
    private readonly TimeSpan _interval;

    private readonly uint _messageCount;

    private uint _counter;

    private ICancelable _counterTimerCancel;

    private readonly TimeSpan _messageSendInterval;

    private ICancelable _messageTimerCancel;

    private bool _messageSent;

    public EvenlyThrottlerActor(TimeSpan interval, uint messageCount)
    {
      _interval = interval;
      _messageCount = messageCount;
      _messageSendInterval = TimeSpan.FromMilliseconds(_interval.TotalMilliseconds / _messageCount);
      Receive<Wrapper<string>>(
        message =>
          {
            SetTimers();
            SendMessage(message);
            Become(WaitingForTrigerToSendNextMessage);
          });

      Receive<ResetCounterTimer>(h => { _counterTimerCancel?.Cancel(); });

      Receive<MessageTimer>(m => { _messageTimerCancel?.Cancel(); });
    }

    public IStash Stash { get; set; }

    private void CreateCancelHandlers()
    {
      _messageTimerCancel?.Cancel();
      _messageTimerCancel = new Cancelable(Context.System.Scheduler);
      _counterTimerCancel?.Cancel();
      _counterTimerCancel = new Cancelable(Context.System.Scheduler);
    }

    private void Sending()
    {
      Receive<Wrapper<string>>(
        a =>
          {
            SendMessage(a);
            Become(WaitingForTrigerToSendNextMessage);
          });
    }

    private void SendMessage(Wrapper<string> message)
    {
      // send message
      message.DestinationActor.Tell(message.Message);
      _counter++;
      _messageSent = true;
    }

    private void SetTimers()
    {
      CreateCancelHandlers();
      Context.System.Scheduler.ScheduleTellRepeatedly(_interval, _interval, Self, new ResetCounterTimer(), Self, _counterTimerCancel);
      Context.System.Scheduler.ScheduleTellRepeatedly(_messageSendInterval, _messageSendInterval, Self, new MessageTimer(), Self, _messageTimerCancel);
    }

    private void WaitingForTrigerToSendNextMessage()
    {
      Receive<ResetCounterTimer>(
        h =>
          {
            _counter = 0;
            if (_messageSent)
            {
              _messageSent = false;
            }
            else
            {
              UnbecomeStacked();
            }

          });

      Receive<MessageTimer>(
        m =>
          {
            Become(Sending);
            Stash.Unstash();
          });

      Receive<Wrapper<string>>(a => { Stash.Stash(); });
    }
  }

  internal class ResetCounterTimer
  {
  }
}
