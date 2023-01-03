# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.7]
## Changed
* Fixed `IScene.SuppressEvent` having no effect on suppressing warnings if used with UGS Analytics.
* Main development editor version is now 2021.3.16f1.

## [1.2.6]
### Changed
* Fixed `Switcher.PerformSceneTransition<T>` possibly crashing on application quit while it is running.

## [1.2.5]
### Changed
* `IScene.ScreenVisitEventPropertyName` is now deprecated in favor of `IScene.ScreenVisitEventParameterName`.

## [1.2.4]
### Added
* Official support for Unity Gaming Services Analytics!

### Changed
* Pinned UniTask to 2.3.1

### Removed
* UnityPackage support. Please use UPM instead.

## [1.2.3]
### Added
* Pre-release support for Unity Gaming Services Analytics!

### Removed
* ADVANCED.md file has been removed. Its content is now in [wiki](https://github.com/Clpsplug/UniSwitcher/wiki).

## [1.2.2]
### Added
* Added equality methods to `BaseScene`

### Changed
* [Breaking] `IScene.GetRawValue()` (as method) is now `IScene.RawValue` (as property.)
* Development Unity version is now 2021.3.

## [1.2.1]
### Added
* UPM Support!
* Changelogs. Past changes are also included.
* Abstract class `BaseScene`, an implementation of `IScene`.  
  Extend this class to focus on avoiding typing scene path in full!

## [1.2.0]
### Added

* Added `StartedFromAnotherSceneMarker`. ([#12](https://github.com/Clpsplug/UniSwitcher/pull/12))

### Changed

* Although this has been the case for the UnityPackage file from the first version, the repository itself no longer contains dependencies.
  Instead, they are downloaded via UPM, so there is no apparent change when you pull the repository to play with this code.

## [1.1.0]
### Changed
* [BREAKING!] `Fire()` is now an asynchronous method.
  * Please change any `void Fire()` to `async UniTask Fire()`

### Deprecated
* `ISceneEntryPoint.IsHeld()` is no longer considered because `Fire()` is asynchronous.  
  It should be deleted and any need to hold the transition should be done using UniTask's task handling

## [1.0.0]
## Added
* Initial Release!