namespace CQRS_EventSourcing
{
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
}