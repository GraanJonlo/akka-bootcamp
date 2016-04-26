using System;
using Akka.Actor;

namespace WinTail
{
    class TailCoordinatorActor : UntypedActor
    {
        public class StartTail
        {
            public string FilePath { get; private set; }

            public IActorRef ReporterActor { get; private set; }

            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }
        }

        public class StopTail
        {
            public string FilePath { get; private set; }

            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
        }

        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = (StartTail) message;
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x =>
                                                                       {
                                                                           if (x is ArithmeticException)
                                                                               return Directive.Resume;

                                                                           if (x is NotSupportedException)
                                                                               return Directive.Stop;

                                                                           return Directive.Restart;
                                                                       });
        }
    }
}
