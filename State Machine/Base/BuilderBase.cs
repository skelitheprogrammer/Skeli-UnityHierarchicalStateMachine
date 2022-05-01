using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuilderBase<T,U>
{
	protected T _state;

	public abstract BuilderBase<T,U> Begin(string name);
	public abstract U BuildEnter(Action enter);
	public abstract U BuildLogic(Action logic);
	public abstract U BuildExit(Action exit);
}