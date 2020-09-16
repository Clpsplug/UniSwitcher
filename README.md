# UniSwitcher

Advanced scene switcher &amp; data propagator for Unity3D

# Requirements

UniSwitcher requires the following plugins to work.  
Versions are the earliest ones I personally known to work, but it is possible that this works in previous versions.

* Unity 2020.1.x
* [Extenject](https://github.com/svermeulen/Extenject) v9.1.0
* [UniTask](https://github.com/Cysharp/UniTask) v2.0.31

# See it in action

Play `Assets/Scenes/SampleScene.unity`. It transitions into `SecondScene.unity` after 3 seconds of transition.

# Basic usages

## Not using Zenject / Extenject?

1. In all the scenes, **place `Scene Context`** provided by Extenject

2. **[Create `Project Context`](https://github.com/modesttree/Zenject#global-bindings-using-project-context)** in `Resources` folder.

## Scene change

1. **Attach `UniSwitcherInstaller`** to `Project Context`. **Place that installer in `Mono Installers` list.**

2. **Copy the following** into a new file. This creates a class which implements `UniSwitcher.Domain.IScene`.
  ```csharp
  using UniSwitcher.Domain;
  public class MyScene: IScene {
    private readonly string _rawValue;
    public MyScene(string rawValue) {
      _rawValue = rawValue;
    }
    
    public string GetRawValue() {
      return _rawValue;
    }
  }
  ```

  ***:tada: At this point, your project is fully ready to use UniSwitcher!***

3. **Create a class extending `UniSwitcher.Switcher`,** which also extends `MonoBehaviour`.
  ```csharp
  using UniSwitcher.Switcher;
  public class Sample: Switcher
  {
    
  }
  ```

4. In this class, you can **call `PerformSceneTransition`.**  
  The most basic usage is to run `ChangeScene(new MyScene("Assets/path/to/scene.unity"))`.
  ```csharp
  public class Sample: Switcher
  {
    // We choose Start() so that you can see the effect immediately
    private void Start() {
      PerformSceneTransition(ChangeScene(new MyScene("Assets/path/to/scene.unity")));
    }
  }
  ```

  ***:tada: Observe that the scene changes to the one you specified!***

## Scene change & data transfer

First, follow everything in the previous section.

1. **Define a class to hold the data.**  
  ```csharp
  using UniSwitcher.Domain;
  public class SampleData
  {
    public int Answer;
    
    public SampleData(int answer)
    {
      Answer = answer;
    }
  }
  ```

2. UniSwitcher will look for **`UniSwitcher.Domain.ISceneEntryPoint`** in the next scene.  
  If found, UniSwitcher thinks that this is an entrypoint of the scene, and calls `Fire()` on it.  
  This **MUST** be a `MonoBehaviour`, and you can only have this up to one per scene.  
  You can receive the data passed from the previous scene by using `[Inject]` on the data type.
  ```csharp
  using UniSwitcher.Domain;
  public class SampleEntryPoint: MonoBehaviour, ISceneEntryPoint
  {
    [Inject] private SampleData _data;
    public void Fire()
    {
      Debug.Log(_data).Answer);
    }
    public bool Validate()
    {
      return data !== null;
    }
    public void OnFailure(Exception e)
    {
      Debug.LogException(e);
    }
    public bool IsHeld()
    {
      return false;
    }
  }
  ```

3. In the _destination_ scene, **create a new `GameObject` and attach the `ISceneEntryPoint`** you just created (e.g. `SampleEntryPoint`) as a component.

4. In the `Switcher` script in the _originating_ scene, perform the scene change **with the data after the final parameter of `ChangeScene()`.**
  ```csharp
  PerformSceneTransition(ChangeScene(new MyScene("Assets/path/to/scene.unity"), new SampleData(42)));
  ```
  
You can now observe the data being logged in the console!



# How do I...

## supress warnings at `PerformSceneTransition`?

Append `.Forget(Debug.LogException)` to the call. These warnings are because `PerformSceneTransition` is an async method.

```csharp
PerformSceneTransition(new MyScene("path/to/scene.unity")).Forget(Debug.LogException);
```

## avoid typing scene paths in full?

You can create static parameter in `MyScene`...

```csharp
public MyScene: IScene {
  public MyScene Scene1 => new MyScene("Assets/path/to/scene.unity");
  ...
```

...and reference it like this:

```csharp
PerformSceneTransition(MyScene.Scene1);
```

# License

MIT license - see [LICENSE.txt](LICENSE.txt)

# TODOs

* Documentation
  * Especially about transition effect
