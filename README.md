# Skeli-HierarchicalStateMachine
This project is Work In Progress and feedback is welcome. 

> The project contains the implementation of both the State Machine and the Hierarchical State Machine for the Unity game engine.

## About
This implementation of the finite state machine greatly simplifies and speeds up prototyping, allowing you to create state machines using code.

The main idea that you need to keep in mind is that a StateMachine is also a state

# StateMachines and States
Each StateMachines and States has their own name that you will assign when building them.

StateMachine can process other StateMachines and States. 
To make StateMachine work you need:

- AddState(State state);
- AddTransition(Transition transition);
- Call UpdateState() in Update loop;

States just holds logic and they don't do something else.

# Transitions
To make link between StateMachines and States you need to feed the StateMachine or StateMachineContext transitions.

Transition consists of condition logic and bool ShouldTransition() method.
ShouldTransition() method executes in StateMachine and StateMachineContext.

> You can leave condition logic empty, it will make transition always return true

Transition constructors:
```cs
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
```

```cs
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
```
ConditionBase is the wrapper class created to pull out the logic into separate class (WIP).


```cs
public Transition(State from, State to)
{
	this.from = from;
	this.to = to;

    _condition = () => true;
}
```

```cs
public Transition(State to, Func<bool> condition)
{
	from = null;
	this.to = to;

	if (condition == null)
	{
		_condition = () => true;
		return;
	}

	_condition = condition;
}
```

# StateMachineContext
In this project I made a decision that if you want to create Hirearchical State Machine, whole process should rule special class - StateMachineContext.

StateMachineContext is the class which implements IStateMachine interface and controls StateMachines, which in turn controls other StateMachines and States. 
> You can still use StateMachines and create Hierarchical State Machines without this class.

```cs
StateMachineContext fsm = new StateMachineContext();
fsm.Init(StateMachine State); // Need to choose from which statemachine context will start

fsm.UpdateState(); // Update context in update loop
```

# StateMachine and State Builders
To be able to create StateMachines and States you need to create their builders first.

```cs
    var stateMachineBuilder = new StateMachineBuilder();
    var stateBuilder = new StateBuilder();
```

This needs to make creation of StateMachine and State much easier.

Example of creation StateMachine:

```cs
var stateMachineBuilder = new StateMachine.StateMachineBuilder();
var stateBuilder = new State.StateBuilder();
    
StateMachine groundedSM = stateMachineBuilder.Begin("Grounded")
	.BuildLogic()
		.WithEnter(() =>
		{
			SetSpeed(Vector3.zero);
			_animation.SetIsGrounded(true);
		})
		.WithTick(() =>
		{
			if (_groundChecker.GroundCheck())
			{
				if (_direction.IsOnSlope)
				{
					_velocity.y = _gravity.SetGroundedGravity();
				}
			}
		})
		.WithExit(() =>
		{
			_animation.SetIsGrounded(false);
		})
	.Build();

groundedSM.SetEntryState(stateName); //From which state should start this StateMachine
	
```
- Begin() - Creates new instance of empty State/StateMachine.
- BuildLogic() - Opens block where u can modify your State/StateMachine
- WithEnter(Action enter) - Build enter logic that will be called when State/StateMachine is entered
- WithTick(Action logic) - Build update logic that will be called each update tick
- WithExit(Action exit)  - Build exit logic that will be called when states are changing
- Build() - Wraps up everything you made in build methods and returns instance of State/StateMachine;

You can combine which combinations of logic you need to build (Enter+Tick, Enter+Exit, etc.).

> I made that kind of builders because I don't want to create new Builder each time.

# Getting Started

Create instances of StateMachineContext, StateMachineBuilder and StateBuilder classes:
```cs
    public class TestScript : MonoBehaviour
    {
        private StateMachineContext _fsm;
    
        private void Awake()
        {
            _fsm = new StateMachineContext();
		    var stateMachineBuilder = new StateMachine.StateMachineBuilder();
		    var stateBuilder = new State.StateBuilder();
        }
    }
```
Then you can start creating StateMachines and States:

```cs
private void Awake()
{
	_fsm = new StateMachineContext();
	var stateMachineBuilder = new StateMachineBuilder();
	var stateBuilder = new StateBuilder();
        
	StateMachine grounded = stateMachineBuilder.Begin("Grounded")
		.BuildLogic()
			.WithEnter(() => 
			{
				Debug.Log("Hello world!");
			})
			.WithTick(() =>
			{
				Debug.Log("Hey, I'm doing something each Update tick!");
			})
			.WithExit(() => 
			{
				Debug.Log("I have to go but I'll leave this message");
			})
		.Build();
            
	State isJumping = stateBuilder.Begin("Jumping")
			.BuildLogic()
				.WithEnter(() => 
				{
                	Debug.Log("Im JUMPING!");
				})
			.Build();

	StateMachine isFalling = stateMachineBuilder.Begin("Falling")
			.BuildLogic()
				.WithTick(() =>
				{
					Debug.Log("Falling Logic");
				})
			.Build();     
}
```
Adds States:
```cs
    _fsm.AddState(grounded);
    grounded.AddState(isJumping);
    _fsm.AddState(isFalling);
```

Then you add transitions:
```cs
    grounded.AddTransition(new Transition(grounded, isJumping, () => _data.isGrounded && _input.IsJumped));
    grounded.AddTransition(new Transition(isJumping, grounded, () => _data.isGrounded));
    grounded.AddTransition(new Transition(isJumping, isFalling, () => !_data.isGrounded));
    _fsm.AddTransition(new Transition(grounded, isFalling, () => !_data.isGrounded));
    _fsm.AddTransition(new Transition(isFalling, grounded, () => _data.isGrounded));
```
Initialize:
```cs
	_fsm.Init(groundedSM);
```

Update StateMachineContext in Update Loop:
```cs
	_fsm.UpdateState();
```

#Contacts
Feel free to send feedback and ask questions by email: dosynkirill@gmail.com
