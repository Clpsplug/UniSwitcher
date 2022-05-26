# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
* UPM Support!
* Changelogs. Past changes are also included.

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