# MiniMp3Bundle

## Build steps
#### Clone repo with submodules
```
git clone https://github.com/jnigames/MiniMp3Bundle
git submodule init
git submodule update
```

#### Build windows
Run `build_win.cmd` to build 64 and 32 bit dlls in the `build/` directory.

#### Build linux
Run `build_linux.sh` on linux (or wsl) with gcc installed. The `libminimp3.so` will be created in the `build` directory.

#### Build managed dlls
The `MiniMp3Managed/` directory contains project files for the managed part of the bundle which uses P/Invoke to interact with the minimp3 library.
Use the sln to create release builds or run:
```
dotnet build MiniMp3Managed.Win64.csproj --configuration Release
dotnet build MiniMp3Managed.Win32.csproj --configuration Release
dotnet build MiniMp3Managed.Posix.csproj --configuration Release
```
The dlls will be located at `MiniMp3Managed/bin/Release/netstandard2.0/`
