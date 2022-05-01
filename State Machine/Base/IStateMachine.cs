public interface IStateMachine
{
    void AddState(State state);
    void AddTransition(Transition transition);
    void ChangeState(State state);
    void UpdateState();
}