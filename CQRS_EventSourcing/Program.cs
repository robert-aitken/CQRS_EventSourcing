using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS_EventSourcing
{
    class Program
    {
        public class Person
        {
            EventBroker broker;
            private int age;

            public Person(EventBroker broker)
            {
                this.broker = broker;
                broker.Commands += BrokerOnCommands;
                broker.Queries += Broker_Queries;
            }

            private void Broker_Queries(object sender, Query e)
            {
                var q = e as AgeQuery;
                if (q != null && q.Target == this)
                {
                    q.Result = age;
                }
            }

            private void BrokerOnCommands(object sender, Command e)
            {
                var cac = e as ChangeAgeCommand;
                if (cac != null && cac.Target == this)
                {
                    if (cac.Registred)
                    {
                        broker.AllEvents.Add(new AgeChangedEvent(this, age, cac.Age));
                    }
                    age = cac.Age;
                }
            }
        }

        public class EventBroker
        {
            //1. All events 2. Commands 3. Queries
            public IList<Event> AllEvents = new List<Event>();
            public event EventHandler<Command> Commands;
            public event EventHandler<Query> Queries;

            public void Command(Command c)
            {
                Commands?.Invoke(this, c);
            }

            public T Query<T>(Query q)
            {
                Queries?.Invoke(this, q);
                return (T)q.Result;
            }

            public void UndoLast()
            {
                var e = AllEvents.LastOrDefault();
                var ac = e as AgeChangedEvent;
                if (ac != null)
                {
                    Command(new ChangeAgeCommand(ac.Target, ac.OldAge) { Registred = false });
                    AllEvents.Remove(e);
                }
            }
        }


        public class ChangeAgeCommand : Command
        {
            public Person Target;
            public int Age;

            public ChangeAgeCommand(Person target, int age)
            {
                Target = target;
                Age = age;
            }
        }

        public class Query
        {
            public object Result;
        }

        public class AgeQuery : Query
        {
            public Person Target;
        }

        public class Command : EventArgs
        {
            public bool Registred = true;
        }

        public class Event
        {

        }

        public class AgeChangedEvent : Event
        {
            public Person Target;
            public int OldAge, NewAge;

            public AgeChangedEvent(Person target, int oldAge, int newAge)
            {
                Target = target;
                OldAge = oldAge;
                NewAge = newAge;
            }

            public override string ToString()
            {
                return $"Age changed from {OldAge} to {NewAge}";
            }
        }

        static void Main(string[] args)
        {
            var eb = new EventBroker();
            var p = new Person(eb);

            eb.Command(new ChangeAgeCommand(p, 123));
            eb.Command(new ChangeAgeCommand(p, 55));
            eb.Command(new ChangeAgeCommand(p, 21));
            eb.Command(new ChangeAgeCommand(p, 3));
            eb.Command(new ChangeAgeCommand(p, 77));

            foreach (var e in eb.AllEvents)
            {
                Console.WriteLine(e);
            }

            int age = eb.Query<int>(new AgeQuery { Target = p });

            Console.WriteLine(age);

            eb.UndoLast();

            age = eb.Query<int>(new AgeQuery { Target = p });
            Console.WriteLine(age);

            Console.ReadKey();
        }
    }
}