# UniSwitcher

Scene switcher &amp; data propagator wrapper for Unity3D. This plugin provides interfaces and classes to handle the following tasks in a controlled manner:
* Parameter propagation between scenes (Wraps Zenject's SceneLoader)
* Displaying **transition effects!**
* **Triggering an initialization task** on the next scene using UniTask
  * and holding the transition while initialization takes place
  * Graceful initialization failure handling to prevent lock-ups
* Scene loading progress display

# Installing

## Using UPM

Since the dependency is provided through git links, we need [mob-sakai/GitDependencyResolverForUnity]("https://github.com/mob-sakai/GitDependencyResolverForUnity).
Add these lines to your `manifest.json`:
```json
{
  "dependencies": {
    "com.clpsplug.uniswitcher": "https://github.com/clpsplug/uniswitcher.git?path=Packages/com.clpsplug.uniswitcher",
    "com.coffee.git-dependency-resolver": "https://github.com/mob-sakai/GitDependencyResolverForUnity.git"
  }
}
```

This way is the easiest since you do not have to get the requirements - it will be done automatically.
If you have the depencencies mentioned below inside your `Plugin` folder, you will need to delete those as you will run into duplicate issues.

## From this repository

Although not recommended, you can copy the content of `Packages/com.clpsplug.uniswitcher`.
If you are taking this route, you need to get the dependencies yourself.

### Requirements & Dependencies

UniSwitcher requires the following plugins to work.  
The versions are the earliest ones I personally checked with; it is possible that this still works with later versions.

* Unity 2020.x
* [Zenject](https://github.com/modesttree/Zenject) v9.1.0
* [UniTask](https://github.com/Cysharp/UniTask) v2.0.31

# See it in action

Play `Assets/Scenes/SampleScene.unity`. It transitions into `SecondScene.unity` after 3 seconds of transition.

# How to Use

➡️ See [the Wiki](https://github.com/clpsplug/UniSwitcher/wiki) for usage!  
For quickstart, see [here](https://github.com/Clpsplug/UniSwitcher/wiki/Quickstart).

# License

MIT license - see [LICENSE.md](LICENSE.md)

# Contributions

Issues and PRs are welcome!
