using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AsyncMain().Wait();
        }

        private static async Task AsyncMain()
        {
            using (var myActorSystem = ActorSystem.Create("MyActorSystem"))
            {
                Props consoleWriterProps = Props.Create<ConsoleWriterActor>();
                IActorRef consoleWriterActor = myActorSystem.ActorOf(consoleWriterProps, "consoleWriterActor");

                Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
                myActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

                Props validationActorProps = Props.Create(() => new FileValidatorActor(consoleWriterActor));
                myActorSystem.ActorOf(validationActorProps, "validationActor");

                Props consoleReaderProps = Props.Create(() => new ConsoleReaderActor());
                IActorRef consoleReadActor = myActorSystem.ActorOf(consoleReaderProps, "consoleReaderActor");

                consoleReadActor.Tell(ConsoleReaderActor.StartCommand);

                await myActorSystem.WhenTerminated;
            }
        }
    }
}
