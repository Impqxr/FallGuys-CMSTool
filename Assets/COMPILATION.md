# CMS Tool compilation guide

## Windows

1. Installation of [Git for Windows](https://git-scm.com/downloads/win) and [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

- You can do this easily with one command in PowerShell or manually

```shell
winget install Microsoft.DotNet.SDK.8 Git.Git --source winget
```

2. Now compile CMS Tool with these commands in PowerShell

```shell
git clone https://github.com/floyzi/FallGuys-CMSTool.git; cd FallGuys-CMSTool
dotnet publish -c Release-Win-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true
```

3. Output files will be located in `.\CMSTool\bin\Release-Win-x64\net8.0\win-x64\publish`

## Linux

1. Installation of .NET SDK 8.0 and Git

Different distributions have different packages in their repositories, below are examples for some of them. For more distributions or details check [this page](https://learn.microsoft.com/en-us/dotnet/core/install/linux) or just google "How to install .NET SDK 8.0 on DISTRONAME"

- Ubuntu

```shell
apt install dotnet-sdk-8.0 git
```

- Fedora

```shell
dnf install dotnet-sdk-8.0 git
```

- Arch Linux

```shell
pacman -S dotnet-sdk-8.0 git
```

2. Now compile CMS Tool with these commands

```shell
git clone https://github.com/floyzi/FallGuys-CMSTool.git; cd FallGuys-CMSTool
dotnet publish -c Release-Linux-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true
```

3. Output files will be located in `./CMSTool/bin/Release-Linux-x64/net-8.0/linux-x64/publish`
