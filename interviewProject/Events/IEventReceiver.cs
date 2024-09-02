namespace interviewProject.Events
{
    public interface IEventReceiver
    {
        void Register();
        void Unregister();
    }
}
