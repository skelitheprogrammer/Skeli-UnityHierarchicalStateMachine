using System;

public class Transition : TransitionBase
{
    public readonly State from;
    public readonly State to;

    private readonly Func<bool> _condition;

    public Transition(State from, State to, Func<bool> condition)
    {
        this.from = from;
        this.to = to;

        if (condition == null)
        {
            _condition = () => true;
            return;
        }

        _condition = condition;

    }

    public Transition(State from, State to, ConditionBase condition, bool isReversed)
    {
        this.from = from;
        this.to = to;

        if (condition == null)
        {
            _condition = () => true;
            return;
        }

        if (isReversed)
        {
            _condition = () => !condition.BoolCondition;
        }
        else
        {
            _condition = () => condition.BoolCondition;
        }
    }

    public Transition(State from, State to)
    {
        this.from = from;
        this.to = to;

        _condition = () => true;
    }

    public override bool ShouldTransition()
    {
        return _condition();
    }
}
