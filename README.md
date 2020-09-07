# UniSwitcher

Advanced scene switcher &amp; data propagator for Unity3D

# Requirements

UniSwitcher requires the following plugins to work.  
Versions are the earliest ones I personally known to work, but it is possible that this works in previous versions.

* Unity 2020.1.x
* [Extenject](https://github.com/svermeulen/Extenject) v9.1.0
* [UniTask](https://github.com/Cysharp/UniTask) v2.0.31

# Basic usages

## Scene change

Start from 3. if you already use Zenject/Extenject.

1. In all the scenes, **place `Scene Context`** provided by Extenject

2. **[Create `Project Context`](https://github.com/modesttree/Zenject#global-bindings-using-project-context)** in `Resources` folder.

3. **Attach `UniSwitcherInstaller`** to that `Project Context` you just added, and **place that installer in `Mono Installers` list.**

4. **Copy the following** into a new file. This creates a class which implements `UniSwitcher.Domain.IScene`.
  ```csharp
  using UniSwitcher.Domain;
  public MyScene: IScene {
    private readonly string _rawValue;
    public MyScene(string rawValue) {
      _rawValue = rawValue;
    }
    
    public string GetRawValue() {
      return _rawValue;
    }
  }
  ```

  ***At this point, your project is fully ready to use UniSwithcer!***

4. **Create a class extending `UniSwitcher.Switcher`,** which also extends `MonoBehaviour`.
  ```csharp
  using UniSwitcher.Switcher;
  public Sample: Switcher
  {
    
  }
  ```

5. In this class, you can **call `PerformSceneTransition`.**  
  The most basic usage is to pass `ChangeScene(new MyScene("Assets/path/to/scene.unity"))`.
  ```csharp
  public Sample: Switcher
  {
    // We choose Start() so that you can see the effect immediately
    private void Start() {
      PerformSceneTransition(ChangeScene(new MyScene("Assets/path/to/scene.unity")));
    }
  }
  ```

Observe that the scene changes to the one you specified!

## Scene change & data transfer

First, follow everything in the previous section.

1. **Define a class that implements `UniSwitcher.Domain.ISceneData`.**  
  There are nothing to implement, but you need to define the schema of data you want to pass.  
  This is done to catch weird data being passed at compile time.
  ```csharp
  using UniSwitcher.Domain;
  public class SampleData: ISceneData
  {
    public int Answer;
    
    public SampleData(int answer)
    {
      Answer = answer;
    }
  }
  ```

2. UniSwitcher will look for **`UniSwitcher.Domain.IDataLoader`** on changing into scenes.   
  This **MUST** be a `MonoBehaviour`, and you need to create one per a scene.
  ```csharp
  using UniSwitcher.Domain;
  public class SampleDataLoader: MonoBehaviour, IDataLoader
  {
    public void Load(ISceneData data)
    {
      Debug.Log(data as SampleData);
    }
    public bool Validate(ISceneData data)
    {
      return data is SampleData;
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

3. In the _destination_ scene, **create a new `GameObject` and attach the `IDataLoader`** you just created (e.g. `SampleDataLoader`) as a component.

4. In the `Switcher` script in the _originating_ scene, perform the scene change **with `.AttachData()` appended to `ChangeScene()`.**
  ```csharp
  PerformSceneTransition(ChangeScene(new MyScene("Assets/path/to/scene.unity")).AttachData(new SampleData(42)));
  ```
  
You can now observe the data begin logged in the console!



# How do I...

## supress warnings at `PerformSceneTransition`?

Append `.Forget(Debug.LogException)` to the call. These warnings are because `PerformSceneTransition` is an async method.

```csharp
PerformSceneTransition(new MyScene("path/to/scene.unity")).Forget(Debug.LogException);
```

## avoid typing scene paths in full?

You can create static members in `MyScene`...

```csharp
public MyScene: IScene {
  public MyScene Scene1 => new MyScene("Assets/path/to/scene.unity");
  ...
```

...and reference it like this:

```csharp
PerformSceneTransition(MyScene.Scene1);
```


# TODOs

* Documentation
  * Especially about transition effect
