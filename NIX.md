# CMS Tool Nix

## Installing an upstream release (Flake) (recommended)

### Installing the package directly

After adding `github:Impqxr/FallGuys-CMSTool` to inputs in your flake, you can access the `packages` output.
Example:

```nix
{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

    cmstool = {
      url = "github:Impqxr/FallGuys-CMSTool";
    };
  };

  outputs =
    { nixpkgs, cmstool, ... }:
    {
      nixosConfigurations.nixos = nixpkgs.lib.nixosSystem {
        modules = [
          ./configuration.nix

          (
            { pkgs, ... }:
            {
              environment.systemPackages = [ cmstool.packages.${pkgs.system} ];
            }
          )
        ];
      };
    };
}
```

### Using commands (Works for any distribution with Nix package manager)

You can simply run these commands to either run or install the default package from this flake.

```shell
# To run it
nix run github:Impqxr/FallGuys-CMSTool

# To install it
nix profile install github:Impqxr-CMSTool-Linux
```

## Installing an upstream release (Without flakes)

### Installing the package directly

We use flake-compat to allow using this without enabled flakes in your system.

```nix
{ pkgs, ... }:
{
  environment.systemPackages = [
    (import (
      builtins.fetchTarball "https://github.com/Impqxr/FallGuys-CMSTool/archive/linux.tar.gz"
    )).packages.${pkgs.system}.default
  ];
}
```

### Using commands to install it (Works for any distribution with Nix package manager)

```shell
nix-channel --add https://github.com/Impqxr/FallGuys-CMSTool/archive/linux.tar.gz cmstool
nix-channel --update cmstool

nix-env -iA cmstool.default
```
