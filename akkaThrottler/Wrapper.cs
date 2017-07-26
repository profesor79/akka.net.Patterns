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

  using Akka.Actor;



  /// <summary>The wrapper.</summary>
  /// <typeparam name="T"></typeparam>
  public class Wrapper<T>
  {
    /// <summary>Initializes a new instance of the <see cref="Wrapper{T}"/> class.</summary>
    /// <param name="message">The message.</param>
    /// <param name="destinationActor">The destination actor.</param>
    public Wrapper(T message, IActorRef destinationActor)
    {
      Message = message;
      DestinationActor = destinationActor;
    }

    /// <summary>Gets the destination actor.</summary>
    public IActorRef DestinationActor { get; }

    /// <summary>Gets the message.</summary>
    public T Message { get; }
  }
}

