# Advanced Usage of UniSwitcher

In addition to `Switcher`, there are several options and classes you can make use of
to create fancy transition effects!


## Additional Configurations

`ChangeScene()` and `AddScene()` are actually builder methods - you can chain the following methods
after them.

### `WithTransitionEffect()`

Performs a transition with an effect. 
[This requires implementing `ITransitionBackgroundController`](#itransitionbackgroundcontroller), 
otherwise it does nothing.

Not calling this method means a hard-cut to the next scene.

### `HideProgressBar()`

If you have a [Progress Display Controller](#progressdisplaycontroller), this causes it to be hidden
for this transition. Useful if you are additively loading another scene.

### `After(float seconds)`

Performs the transition after specified seconds.

## Advanced `ISceneEntryPoint` Implementation

When the new scene starts, you usually want to run some initialization code.  
You would want to do it in `Start()`, but UniSwitcher provides `ISceneEntryPoint.Fire()` that can handle
several (not uncommon) special cases.

### Handling initialization errors

If any initialization error occurs in your `Fire()` method, `OnFailure(Exception e)` is called.
You could just log the exception here, but depending on how you implement your scene,
this may cause your game to softlock, which is bad.  
Using `OnFailure`, you can implement a user-friendly fail-safe method!

```csharp
public async UniTask Fire() {
    throw new Exception("Initialization Error!");
}

public void OnFailure(Exception e) {
    Debug.LogException(e);
    // You can display a UI element and/or activate fail-safe here
    // so that the game does not softlock and that the player knows what just happened.
}
```

### Holding the transition until the initialization ends

You can hold the game in the "transitioning" state while the game is initializing the scene,
e.g., writing the saves.  
If you want to do this, it is _strongly_ recommended 
that you [implement the "Scene transition effect"](#itransitionbackgroundcontroller),
otherwise you could allow the player to interact with the uninitialized scene!

This is why the `Fire()` method is meant to be an async method. 
The benefit of this is that the game does not freeze while you run a long initialization task, 
as long as you put them in `await`.

```csharp
public async UniTask Fire() {
    var result = await SendRecordToServer(sceneData); // may take very long, but the game won't freeze.
    sceneElement.Initialize(result); // Use the result to initialize the scene
}
```



### Asserting the data

You can assert the data sent to the Scene. This is meant to be used by the developer.  
Implement `Validate()` and return `false` when something is not right about the data.

> **[IMPORTANT]**  
> Unlike `Fire()` and `OnFailure()`, if this fails, the initializatino progress will totally halt
> as `ArgumentException` is thrown!  
> Do _not_ use this method to handle errors that may occur during normal gameplay.




## Advanced Classes

These classes, if implemented, add to the base transition effect - super recommended to implement those!

### ProgressDisplayController

This MonoBehaviour is a class to make a scene progress bar.

This progress bar is meant to be placed in each scene where it's needed.
(do not put it in a global, DontDestroyOnLoad GameObject - it may behave in unexpected way)

> **[IMPORTANT!]**  
> For your implementation to work, you **must** reference the progress bar in the scene from
> your `Switcher`'s `sceneProgressBarController` property!

#### `SetProgress(float progress)`

`Switcher` will send the scene load progress to this method.
You probably want to set the parameter `progress` to e.g., Image's fillAmount.

#### `Enable(bool reset = true)`

Called when the progress bar is needed.
If `reset` is true, directly set the displayed progress to zero.

#### `Disable()`

Called when the progress bar is not needed anymore.  

> **[NOTE!]**  
> Do _not_ destroy the progress bar instance here!
> This method is meant to merely **hide** the progress bar from view.
> Destroying the progress bar is done in a separate method.
 
#### `SetDDoL()`

Called when the scene load is complete. Set the progress bar as `DontDestroyOnLoad()` in this method.  
This is required as some progress bar may want to show `100%` after the scene loads;
but attempts to do that without DDoL will fail, because by the time `100%` is shown,
the scene would have changed, and the progress bar that was on the original scene is
destroyed (which will crash the game; we call this the phantom `100%`.)  
This makes sure that the progress bar in the original scene lives after the scene change
just to make sure `100%` can be seen.


#### `Close()`

Called when the scene change is complete. You **must** implement this method as an `async` method,
and you **must** destroy the ProgressBar GameObject here.  
This is used in conjunction with `SetDDoL()`. Due to the phantom `100%`, the progress bar is
in DDoL state after the scene load. `Switcher` will call `Close()` along with `SetDDoL()`
so that the progress bar is correctly cleaned up.  
Since this method is an async method, you can cause the bar to disappear after an animation.

```csharp
public override async UniTask Close() {
    animator.SetTrigger("FadeOut"); // A hyphothetical animation that takes a second to complete
    await UniTask.Delay(TimeSpan.FromSeconds(1f));
    Destroy(gameObject);
}
```

### ITransitionBackgroundController

Implement this Interface to enable transition effect!

In UniSwitcher, the transition is defined in four states, which is defined in `TransitionState` enum.

|State Name|Description|
|:---:|:---|
|Ready|The transition is not started, i.e., waiting. This is the default state. <br/>Also reaches this state when the "Out" state ends.|
|In|Requires an explicit interaction to switch to this state. <br/>This is where the previous scene is starting to get covered with the transition screen.|
|Hold|When the "In" state ends, the transition state automatically switches to this state.<br/>Only the transition screen can be seen by the user.|
|Out|Requires an explicit interaction to switch to this state. <br/>This is where the next scene is starting to appear below the transition screen.|

Therefore, the transition effect _should_ have an Animator which has the four states above.

**The transition effect for UniSwitcher is assumed to be a Canvas prefab**.  
After implementing this script,
create a Canvas, attach the implementation, and save it as a prefab.

> **[IMPORTANT]**  
> For your transition effect to work, you **must** reference the prefab from
> your `UniSwitcherInstaller`! If you are reading this, you should already have
> attached `UniSwitcherInstaller` to your `Project Context`. Reference your transition effect object
> from that one.
>
> Also, note that you can set one and only one transition effect prefab throughout the project.


#### `DetectMainCamera()`

If your Canvas is using `Screen space - Camera` mode, you need to poll the current `Camera.main` and 
update the canvas camera to that one.

#### `TriggerTransitionIn<T>(Switcher.SceneTransitionConfiguration<T> config)`

Called when the Transition In is needed. Put the transition into the "In" state.

#### `TriggerTransitionOut<T>(Switcher.SceneTransitionConfiguration<T> config);`

Called when the Transition Out is needed, Put the transitino into the "Out" state.

#### `ForceTransitionWait()`

Called for a rare case where you want to force the transition state to `Ready` state.  

> **[TIP]**  
> You could create a transition in your Animator so that you can handle this case,
> but this is only called when `Switcher.ForceTransitionWait`. 
> If you are not using that, you can implement this method as no-op.

#### `GetTransitionState()`

Report the current transition state as `TransitionState` enum. 
This probably requires polling your Animator, such as:

```csharp
var stateHash = transitionAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
if (stateHash == Animator.StringToHash("Base Layer.TransitionIn")) {
    return TransitionState.In;
} else if ( // and so on
```


## Extending the Scene Transition Configuration


