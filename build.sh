export VINTAGE_STORY=/var/lib/flatpak/app/at.vintagestory.VintageStory/x86_64/stable/active/files/extra/vintagestory
dotnet run --project ./CakeBuild/CakeBuild.csproj -- "$@"
