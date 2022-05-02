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

# StateMachineContext
In this project I made a decision that if you want to create Hirearchical State Machine, whole process should rule special class - StateMachineContext.

StateMachineContext is the class which implements IStateMachine interface and controls StateMachines, which in turn controls other StateMachines and States. 

# StateMachine and State Builders
To be able to create StateMachines and States you need to create their builders first.

```cs
    var stateMachineBuilder = new StateMachine.StateMachineBuilder();
    var stateBuilder = new State.StateBuilder();
```

This needs to make creation of StateMachine and State much easier.

Example of creation StateMachine:

```cs
    var stateMachineBuilder = new StateMachine.StateMachineBuilder();
    var stateBuilder = new State.StateBuilder();
        
    StateMachine grounded = stateMachineBuilder.Begin("Grounded")
	    .BuildEnter(() => 
	    {
            Debug.Log("Hello world!");
	    })
	    .BuildLogic(() =>
	    {
		    Debug.Log("Hey, I'm doing something each Update tick!");
        })
	    .BuildExit(() => 
	    {
		    Debug.Log("I have to go but I'll leave this message");
	    })
	    .Build();
```
- Begin() - Creates new instance of empty State/StateMachine.
- BuildEnter(Action enter) - Build enter logic that will be called when State/StateMachine is entered
- BuildLogic(Action logic) - Build update logic that will be called each update tick
- BuildExit(Action exit)  - Build exit logic that will be called when states are changing
- Build() - Wraps up everything you made in build methods and returns instance of State/StateMachine;

You can combine which combinations of logics you need to build (Enter+Logic, Enter+Exit, etc.).

> It's important to start creation of you next State/StateMachine with Begin() method to ensure that you will create new instance of State/StateMachine.
> I made that because I don't want to create new Builder each time.


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
		var stateMachineBuilder = new StateMachine.StateMachineBuilder();
		var stateBuilder = new State.StateBuilder();
        
		StateMachine grounded = stateMachineBuilder.Begin("Grounded")
			.BuildEnter(() => 
			{
                Debug.Log("Hello world!");
			})
			.BuildLogic(() =>
			{
				Debug.Log("Hey, I'm doing something each Update tick!");
			})
			.BuildExit(() => 
			{
				Debug.Log("I have to go but I'll leave this message");
			})
			.Build();
            
		State isJumping = stateBuilder.Begin("Jumping")
			.BuildEnter(() =>
			{
			})
			.Build();

		StateMachine isFalling = stateMachineBuilder.Begin("Falling")
			.BuildEnter(() => 
			{
			})
			.BuildLogic(() =>
			{
			})
			.BuildExit(() => 
			{
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
Initialize StateMachineContext:
```cs
	_fsm.Init();
```

Update StateMachineContext in Update Loop:
```cs
	_fsm.UpdateState();
```

## TODO's
- Add Transitions from any
- Try to combine StateMachine and State builders into 1 realization
