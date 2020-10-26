using System;

namespace CQRS_EventSourcing
{
    class Program
    {
        static void Main(string[] args)
        {
            var eb = new EventBroker();
            var p = new Person(eb);

            Console.WriteLine("About to change Age...");
            MakeChangesToAge(eb, p);

            Console.WriteLine("About to check last value of Age...");
            CheckLastValueOfAge(eb, p);

            Console.WriteLine("About to undo last set value to Age...");
            UndoLastSetValueToAge(eb, p);

            Console.ReadKey();
        }

        private static void UndoLastSetValueToAge(EventBroker eb, Person p)
        {
            eb.UndoLast();
            int age = eb.Query<int>(new AgeQuery { Target = p });
            Console.WriteLine($"Undone the changes of age. Age is now: {age}");
        }

        private static void CheckLastValueOfAge(EventBroker eb, Person p)
        {
            int age = eb.Query<int>(new AgeQuery { Target = p });
            Console.WriteLine($"The last Age to be set is: {age}");

            Console.WriteLine("\n-----------------------------------\n");
        }

        private static void MakeChangesToAge(EventBroker eb, Person p)
        {
            eb.Command(new ChangeAgeCommand(p, 123));
            eb.Command(new ChangeAgeCommand(p, 55));
            eb.Command(new ChangeAgeCommand(p, 21));
            eb.Command(new ChangeAgeCommand(p, 3));
            eb.Command(new ChangeAgeCommand(p, 77));

            foreach (var e in eb.AllEvents)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("\n-----------------------------------\n");
        }
    }
}