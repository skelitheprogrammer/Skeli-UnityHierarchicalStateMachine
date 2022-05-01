using System;
using UnityEngine;

public class State : StateBase
{
	protected Action OnEnter;
	protected Action OnLogic;
	protected Action OnExit;

	public readonly string name;

	public override void Enter() => OnEnter?.Invoke();
	public override void DoLogic() => OnLogic?.Invoke();
	public override void Exit() 
	{
		OnExit?.Invoke();
	}

	public State(string name)
	{
		this.name = name;
	}

	public class StateBuilder : BuilderBase<State, StateBuilderFinal>
	{
		public override BuilderBase<State, StateBuilderFinal> Begin(string name)
		{
			_state = new State(name);
			return this;
		}

		public override StateBuilderFinal BuildEnter(Action enter)
		{
			_state.OnEnter = enter;
			return new StateBuilderFinal(_state);
		}

		public override StateBuilderFinal BuildExit(Action exit)
		{
			_state.OnExit = exit;
			return new StateBuilderFinal(_state);
		}

		public override StateBuilderFinal BuildLogic(Action logic)
		{
			_state.OnLogic = logic;
			return new StateBuilderFinal(_state);
		}
	}
	
	public sealed class StateBuilderFinal : StateBuilder
	{
		public StateBuilderFinal(State state)
        {
			_state = state;
        }

		public State Build() => _state;
	}

}