namespace Skeli.StateMachine
{

    public interface IStateMachine
    {
        void AddState(State state);
        void AddTransition(Transition transition);
        void UpdateState();
    }
}