# akka.net.Patterns
Common actor patterns which have been found to be useful, elegant or instructive. 


Throttling Messages
Contributed by: Kaspar Fischer
“A message throttler that ensures that messages are not sent out at too high a rate.”

The pattern is described in Throttling Messages in Akka 2.

Balancing Workload Across Nodes
Contributed by: Derek Wyatt

The pattern is described Balancing Workload across Nodes with Akka 2.

Work Pulling Pattern to throttle and distribute work, and prevent mailbox overflow
Contributed by: Michael Pollmeier

The pattern is described Work Pulling Pattern to prevent mailbox overflow, throttle and distribute work.

Ordered Termination
Contributed by: Derek Wyatt

"When an Actor stops, its children stop in an undefined order. Child termination is asynchronous and thus non-deterministic.
The pattern is described An Akka 2 Terminator.

Akka AMQP Proxies
Contributed by: Fabrice Drouin
The pattern is described Akka AMQP Proxies.

Shutdown Patterns in Akka 2
Contributed by: Derek Wyatt
The pattern is described Shutdown Patterns in Akka 2.

Distributed (in-memory) graph processing with Akka
Contributed by: Adelbert Chang
The pattern is described Distributed In-Memory Graph Processing with Akka.

Case Study: An Auto-Updating Cache Using Actors
Contributed by: Eric Pederson
The pattern is described Case Study: An Auto-Updating Cache using Actors.

Discovering message flows in actor systems with the Spider Pattern
Contributed by: Raymond Roestenburg
The pattern is described Discovering Message Flows in Actor System with the Spider Pattern.

Scheduling Periodic Messages
See Actor Timers