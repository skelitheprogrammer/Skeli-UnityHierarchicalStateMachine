public class Condition : ConditionBase
{
    public override bool BoolCondition { get; }

    public Condition(bool condition)
    {
        BoolCondition = condition;
    }
}
