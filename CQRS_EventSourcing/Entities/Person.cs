namespace CQRS_EventSourcing
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
}