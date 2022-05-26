# UniSwitcher

Scene switcher &amp; data propagator wrapper for Unity3D

# Installing

## Using UPM

Add this line to your `manifest.json`
```
  "com.clpsplug.uniswitcher": "https://github.com/clpsplug/uniswitcher.git"
```

This way is the easiest since you do not have to get the requirements - it will be done automatically.
If you have those depencencies inside `Plugin` folder, you will need to delete those as you will run into duplicate issues.

## Using Unity Package

Head over to [the releases page](https://github.com/Clpsplug/UniSwitcher/releases) and download the latest .unitypackage file.  
If you are taking this route, you will need to download the requirements yourself.

### Requirements

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

MIT license - see [LICENSE.txt](LICENSE.txt)

# Contributions

Issues and PRs are welcome!
