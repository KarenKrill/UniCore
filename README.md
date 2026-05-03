# UniCore

A set of libraries that speed up Unity project development

### Installation

To install UniCore, you can use the Unity Package Manager. Add the following URL to your package manager's "Install package from git URL" option:
```
https://github.com/KarenKrill/UniCore.git?path=Assets/UniCore#1.0.0
```
Depending on the version you want to use, you can change the version tag at the end of the URL.
For example, to use version 1.0.0, you would use the URL above. For the latest version, you can omit the version tag.

### Dependencies

UniCore requires the following packages to be pre-installed:

- UniTask
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

In the future, I'll try to move dependent functionality into separate packages so you don't have to drag in a bunch of unnecessary frameworks :D