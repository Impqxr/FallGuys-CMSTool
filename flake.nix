{
  description = "Flake for FallGuys-CMSTool";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs";
    flake-compat = {
      url = "github:edolstra/flake-compat";
      flake = false;
    };
  };

  outputs = { self, nixpkgs, ... }:
    let
      # For future purposes
      systems = [ "x86_64-linux" ];
      forAllSystems = f: nixpkgs.lib.genAttrs systems (system: f system);
      pkgsFor = system: import nixpkgs { inherit system; };
    in
    {
      packages = forAllSystems (system:
        let
          pkgs = pkgsFor system;
        in
        {
          default = pkgs.buildDotnetModule rec {
            name = "FallGuys-CMSTool";
            version =
              let
                # Someone help me to figure out the way to get the version normally or make dotnet see .git directory
                projectVersion = (builtins.elemAt (builtins.match ".*<Version>([a-zA-Z0-9._-]+)</Version>.*" (builtins.readFile "${self}/${projectFile}")) 0);
              in
              "${projectVersion}+${self.rev or self.dirtyRev or "unknown"}";

            nativeBuildInputs = with pkgs; [
              copyDesktopItems
            ];

            src = ./.;
            projectFile = "CMSTool/FallGuys-CMSTool.csproj";
            dotnet-sdk = pkgs.dotnetCorePackages.sdk_8_0;
            dotnet-runtime = pkgs.dotnetCorePackages.runtime_8_0;
            nugetDeps = ./deps.json;
            executables = [ "CMSTool" ];

            desktopItems = [
              (pkgs.makeDesktopItem {
                name = "CMSTool";
                desktopName = "CMSTool";
                comment = "An easy to use tool to decrypt and encrypt Fall Guys content files.";
                exec = "CMSTool";
                icon = "cmstool";
                categories = [ "Utility" ];
                terminal = false;
              })
            ];

            fixupPhase = ''
              runHook preFixup

              for size in 16x16 32x32 48x48 64x64 128x128 256x256 512x512; do
                install -D "$src/CMSTool/Assets/icons/avalonia-logo-$size.png" "$out/share/icons/hicolor/$size/apps/cmstool.png"
              done

              runHook postFixup
            '';
          };
        });

      apps = forAllSystems (system: {
        default = {
          type = "app";
          program = "${self.packages.${system}.default}/bin/CMSTool";
        };
      });

      devShells = forAllSystems (system:
        let
          pkgs = pkgsFor system;
        in
        {
          default = pkgs.mkShell {
            packages = with pkgs; [
              dotnetCorePackages.sdk_8_0
              nuget-to-json
              xorg.libX11
              xorg.libICE
              xorg.libSM
              fontconfig
            ];

            shellHook = ''
              export LD_LIBRARY_PATH=${pkgs.lib.makeLibraryPath [
                pkgs.fontconfig
                pkgs.xorg.libX11
                pkgs.xorg.libICE
                pkgs.xorg.libSM
              ]}:$LD_LIBRARY_PATH
            '';
          };
        });
    };
}
