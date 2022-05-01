using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineContext : IStateMachine
{
	private readonly List<StateMachine> _stateMachines = new List<StateMachine>();
    private readonly List<Transition> _transitions = new List<Transition>();
	public StateMachine ActiveStateMachine { get; private set; }

    public void AddState(State state)
    {
		_stateMachines.Add((StateMachine)state);
    }

    public void ChangeState(State state)
    {
		ActiveStateMachine?.Exit();
		ActiveStateMachine = (StateMachine)state;
		ActiveStateMachine?.Enter();
    }

    public void UpdateState()
    {
		if (ActiveStateMachine == null) throw new NullReferenceException($"Initialize State Machine Context");

		ActiveStateMachine.UpdateState();

		foreach (var transition in _transitions)
		{
			if (ActiveStateMachine != transition.from)
			{
				continue;
			}

			TryProceedTransition(transition);
		}
	}

	public void Init()
    {
		foreach (var transition in _transitions)
		{
			TryProceedTransition(transition);
		}
	}

    public void AddTransition(Transition transition)
    {
        _transitions.Add(transition);
    }

	private void TryProceedTransition(Transition transition)
	{
		if (transition.ShouldTransition())
		{
			Debug.Log($"Changing {transition.from.name} {transition.to.name}");
			ChangeState(transition.to);
		}
	}
}