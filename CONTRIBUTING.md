# Contributing

Conventions for this Unity project (Unity 6000.3.9f1). Formatting rules are enforced by `.editorconfig`; git handling by `.gitattributes`.

## C# Style

### Formatting
- **Indentation:** tabs, displayed as 4 spaces.
- **Braces:** Allman — every opening brace on its own line.
- **Always use braces**, even for single-line `if`/`for`/`while`.
- **Line endings:** CRLF for `.cs` files (handled by `.gitattributes`).
- **Encoding:** UTF-8 without BOM for all files (enforced by `.editorconfig`). Do not commit UTF-16, ANSI/Windows-1252, or BOM-prefixed files.
- `using` directives outside the namespace, `System.*` first.
- Block-scoped namespaces.

### Types
- **Always explicit types. Never `var`.**
- Use C# keywords (`int`, `string`) over `Int32`, `String`.
- Always write access modifiers (`private` included).

### Naming

| Symbol | Style | Example |
|---|---|---|
| Class, struct, enum, delegate | PascalCase | `PacketBuffer` |
| Interface | `I` + PascalCase | `ITransport` |
| Type parameter | `T` + PascalCase | `TMessage` |
| Method, property, event | PascalCase | `SendPacket()`, `IsConnected` |
| Public / protected / internal field | PascalCase | `MaxPlayers` |
| Private field | camelCase, no prefix | `connectionId` |
| Constant | PascalCase | `DefaultPort` |
| Local, parameter | camelCase | `bytesRead` |

Prefer `[SerializeField] private` over public fields for inspector values.

### Example

```csharp
using System;
using UnityEngine;

namespace RootNamespace.Networking
{
	public sealed class ConnectionMonitor : MonoBehaviour
	{
		private const float DefaultTimeout = 5f;

		[SerializeField] private float timeout = DefaultTimeout;

		private float lastHeartbeat;

		public bool IsAlive { get; private set; }

		private void Update()
		{
			float elapsed = Time.time - lastHeartbeat;
			if (elapsed > timeout)
			{
				IsAlive = false;
			}
		}
	}
}
```

### Unity null checks
Do not use `?.` or `??` on `UnityEngine.Object` types — they bypass Unity's overloaded `==` and miss destroyed objects. Use explicit `!= null` checks.

## Project Structure

- **Root namespace:** _TBD_. Sub-namespaces mirror feature folders (`<Root>.Networking`, `<Root>.Gameplay`, ...).
- **Assembly definitions:** one per feature, each split by role:

```
Assets/_Project/
	Networking/
		Runtime/   <Root>.Networking.asmdef
		Editor/    <Root>.Networking.Editor.asmdef   (Editor platform only)
		Tests/     <Root>.Networking.Tests.asmdef    (test assembly)
	Gameplay/
		Runtime/ ...
```

- Asmdef name = namespace = folder path. Set **Root Namespace** on each asmdef.
- Features depend downward only; no circular references between asmdefs.

## Git

### Workflow
- Solo development, commit directly to `master`.

### Commit messages — Conventional Commits

```
<type>(<scope>): <subject>

<optional body: why, not what>
```

- **Types:** `feat`, `fix`, `refactor`, `perf`, `test`, `docs`, `chore`, `build`, `ci`, `style`, `revert`
- **Scope:** feature/asmdef name, lowercase (`net`, `gameplay`, `ui`, `editor`)
- Subject: imperative mood, lowercase, no trailing period, ≤ 72 chars
- Breaking change: `feat(net)!: ...` plus a `BREAKING CHANGE:` footer

Examples:
```
feat(net): add reliable ordered channel
fix(net): drop packets exceeding mtu instead of throwing
chore: upgrade input system to 1.14
```

### Git LFS
Binary assets (textures, models, audio, video, fonts, native libs, archives) go through LFS — see `.gitattributes`. Run once per clone:

```
git lfs install
```

Check a file is tracked: `git check-attr filter -- path/to/file.png`

### Unity settings required
- **Edit > Project Settings > Editor > Asset Serialization Mode:** Force Text
- **Edit > Project Settings > Editor > Version Control Mode:** Visible Meta Files
- **Edit > Project Settings > Editor > Line Endings For New Scripts:** Windows (keeps new scripts CRLF on every OS)

## Cross-Platform Setup (Windows + macOS)

Line endings and encoding are owned by `.gitattributes` and `.editorconfig`, so results are identical on both OSes. Do not rely on `core.autocrlf`; keep it `false` (Windows) or unset/`input` (macOS).

### Windows — Visual Studio
- Install the **Game development with Unity** workload (`.vsconfig` lists it; Visual Studio Installer prompts on open).
- Visual Studio reads `.editorconfig` natively: tabs, Allman braces, UTF-8 (no BOM), CRLF, naming and `var` rules.
- Unity: **Preferences > External Tools > External Script Editor:** Visual Studio.
- `git config --global core.longpaths true` (avoids MAX_PATH failures in deep Library/Packages paths).

### macOS — VS Code
- Install the recommended extensions when prompted (`.vscode/extensions.json`): **Unity**, **C# Dev Kit**, **EditorConfig**. Without the EditorConfig extension, VS Code ignores `.editorconfig`; `.vscode/settings.json` mirrors the key rules as a fallback.
- C# Dev Kit applies the `.editorconfig` formatting, naming and `var` rules through Roslyn, same as Visual Studio.
- Unity: **Preferences > External Tools > External Script Editor:** Visual Studio Code.
- Git LFS: `brew install git-lfs && git lfs install`.

### Verify on a fresh clone
```
git ls-files --eol -- "*.cs"     # expect i/lf  w/crlf
git ls-files --eol -- "*.unity"  # expect i/lf  w/lf
git lfs ls-files                 # binaries listed
```
- Always commit `.meta` files together with their asset.

### Scene / prefab merges (UnityYAMLMerge)
`.gitattributes` marks Unity YAML files with `merge=unityyamlmerge`. The driver must be defined in git config (not committed). For command-line git:

Windows:
```
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "'H:/UnityEditors/6000.3.9f1/Editor/Data/Tools/UnityYAMLMerge.exe' merge -h -p --force %O %B %A %A"
git config merge.unityyamlmerge.recursive binary
```

macOS (Unity Hub default install path):
```
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "'/Applications/Unity/Hub/Editor/6000.3.9f1/Unity.app/Contents/Tools/UnityYAMLMerge' merge -h -p --force %O %B %A %A"
git config merge.unityyamlmerge.recursive binary
```

GitKraken uses its own merge engine and may not invoke custom merge drivers; for a conflicting scene/prefab, resolve via command-line `git merge` or use `UnityYAMLMerge.exe` as external merge tool.
