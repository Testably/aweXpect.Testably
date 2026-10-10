# File system

You can verify the files, directories and drives of a `MockFileSystem`, either through the `IFileSystem` or directly on
an `IFileInfo`, `IDirectoryInfo`, `IDriveInfo` or `IFileVersionInfo`.

The samples use the `TimeSpan` helpers (`1.Second()`) from
[aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

## Files, directories and drives

You can verify that a file, directory or drive exists in an `IFileSystem`, or that it does not:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).HasDirectory("beatles/abbey-road");
await Expect.That(fileSystem).HasFile("let-it-be.txt");

await Expect.That(fileSystem).DoesNotHaveDirectory("beatles/white-album");
await Expect.That(fileSystem).DoesNotHaveFile("yesterday.txt");
```

For example, `Expect.That(fileSystem).HasFile("yesterday.txt")` fails with:

```text title="Failure message"
Expected that fileSystem
has file 'yesterday.txt',
but it did not exist
```

### File content and timestamps

You can verify the content of the file found by `HasFile(path)`:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotEqualTo("let it go");
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent(Encoding.UTF8.GetBytes("let it be"));
```

The string content supports the same options as
[comparing strings](https://docs.testably.org/aweXpect/values/string#equality). When the content differs, the failure
message shows where and includes the file content:

```text title="Failure message"
Expected that fileSystem
has file 'let-it-be.txt' with content equal to "let it go",
but it was "let it be", which differs at index 7:
          ↓ (actual)
  "let it be"
  "let it go"
          ↑ (expected)

File content:
let it be
```

You can also compare the content with another file on the same file system:

```csharp
fileSystem.File.WriteAllText("let-it-be-remastered.txt", "LET IT BE");
fileSystem.File.WriteAllText("hey-jude.txt", "hey jude");

await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotSameAs("hey-jude.txt");
```

You can verify the timestamps of the file, and widen the comparison to a window with `.Within(tolerance)`:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastWriteTime(DateTime.Now).Within(1.Second());
```

### Directory content

You can verify the files and subdirectories of the directory found by `HasDirectory(path)`:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.Directory.CreateDirectory("beatles/revolver");
fileSystem.File.WriteAllText("beatles/abbey-road/something.txt", "something");

await Expect.That(fileSystem).HasDirectory("beatles").WithDirectories(d => d.HasCount().EqualTo(2));
await Expect.That(fileSystem).HasDirectory("beatles/abbey-road").WithFiles(f => f
    .All().ComplyWith(x => x.HasContent("SOMETHING").IgnoringCase()));
```

### Continuing with the file, directory or drive

`HasFile`, `HasDirectory` and `HasDrive` each have a `.Which` property that continues with the found `IFileInfo`,
`IDirectoryInfo` or `IDriveInfo`, so that the expectations of the following sections can be used in the same chain:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9).And.HasContent("let it be");
await Expect.That(fileSystem).HasDirectory("beatles/revolver").Which.IsEmpty();
await Expect.That(fileSystem).HasDrive("D:\\").Which.IsReady().And.HasDriveFormat("NTFS");
```

## File

You can verify the properties of an `IFileInfo`:

```csharp
IFileInfo fileInfo = fileSystem.FileInfo.New("let-it-be.txt");

await Expect.That(fileInfo).Exists();
await Expect.That(fileInfo).DoesNotExist();

await Expect.That(fileInfo).HasName("let-it-be.txt");
await Expect.That(fileInfo).HasExtension(".txt");
await Expect.That(fileInfo).HasLength(9);
await Expect.That(fileInfo).HasContent("let it be");
await Expect.That(fileInfo).HasContent(Encoding.UTF8.GetBytes("let it be"));

await Expect.That(fileInfo).IsReadOnly();
await Expect.That(fileInfo).IsNotReadOnly();

await Expect.That(fileInfo).HasAttribute(FileAttributes.ReadOnly);
await Expect.That(fileInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await Expect.That(fileInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(fileInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(fileInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

For example, `Expect.That(fileInfo).HasLength(10)` fails with:

```text title="Failure message"
Expected that fileInfo
has length 10,
but it was 9
```

`HasAttribute` and `DoesNotHaveAttribute` check whether the flag is contained, so
`FileAttributes.ReadOnly | FileAttributes.Hidden` satisfies `HasAttribute(FileAttributes.ReadOnly)`. The empty
(`default`) value throws an `ArgumentException`, because it would be contained in every value.

On .NET 10 or later, `WhoseParent` continues with the containing directory, so that the
[directory](#directory) expectations can be used:

```csharp
await Expect.That(fileInfo).WhoseParent.HasName("beatles").And.IsNotEmpty();
```

## Directory

You can verify the properties of an `IDirectoryInfo`:

```csharp
IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("beatles");

await Expect.That(dirInfo).Exists();
await Expect.That(dirInfo).DoesNotExist();

await Expect.That(dirInfo).HasName("beatles");

await Expect.That(dirInfo).IsEmpty();
await Expect.That(dirInfo).IsNotEmpty();

await Expect.That(dirInfo).HasFile("abbey-road/something.txt");
await Expect.That(dirInfo).DoesNotHaveFile("abbey-road/yesterday.txt");
await Expect.That(dirInfo).HasDirectory("abbey-road").Which.HasFile("something.txt");
await Expect.That(dirInfo).DoesNotHaveDirectory("white-album");

await Expect.That(dirInfo).HasAttribute(FileAttributes.Directory);
await Expect.That(dirInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await Expect.That(dirInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(dirInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(dirInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

`HasFile` and `HasDirectory` resolve the path relative to the directory and continue like on the
[file system](#files-directories-and-drives), so `.WithContent(…)`, `.WithLastWriteTime(…)`, `.Which` etc. work as
well.

On .NET 10 or later, `WhoseParent` continues with the parent directory:

```csharp
await Expect.That(dirInfo).WhoseParent.HasName("…");
```

## Drive

You can verify the properties of an `IDriveInfo`:

```csharp
MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));

IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

await Expect.That(driveInfo).HasAvailableFreeSpace(2048);
await Expect.That(driveInfo).HasTotalSize(2048).And.HasTotalFreeSpace(2048);
await Expect.That(driveInfo).HasDriveFormat("NTFS");
await Expect.That(driveInfo).HasDriveType(DriveType.Fixed);
await Expect.That(driveInfo).HasName(driveInfo.Name).And.HasVolumeLabel(driveInfo.VolumeLabel);
await Expect.That(driveInfo).IsReady();
```

`HasDrive` on the file system matches the drive name case-insensitively against `IFileSystem.DriveInfo.GetDrives()`.
UNC drives are not returned by `GetDrives()` and are therefore not supported by `HasDrive`.

## File version info

You can verify an `IFileVersionInfo` obtained via `MockFileSystem.FileVersionInfo.GetVersionInfo`. Its values come
from `MockFileSystem.WithFileVersionInfo(glob, builder)`:

```csharp
MockFileSystem fileSystem = new();
fileSystem.WithFileVersionInfo("*.dll", v => v
    .SetCompanyName("Apple Corps")
    .SetProductName("Abbey Road")
    .SetFileVersion("1.9.6.9")
    .SetIsDebug(true));
fileSystem.File.WriteAllText("AbbeyRoad.dll", "");

IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("AbbeyRoad.dll");

await Expect.That(info).HasCompanyName("Apple Corps").And.HasProductName("Abbey Road");
await Expect.That(info).HasFileVersion("1.9.6.9").And.HasFileMajorPart(1);
await Expect.That(info).IsDebug().And.IsNotPreRelease();
```

For example, `Expect.That(info).HasCompanyName("EMI")` fails with:

```text title="Failure message"
Expected that info
has company name equal to "EMI",
but it was "Apple Corps", which differs at index 0:
   ↓ (actual)
  "Apple Corps"
  "EMI"
   ↑ (expected)
```

Every `IFileVersionInfo` property has its own expectation: `Has…(string)` for the strings, `Has…(int)` for the version
parts and an `Is…()` / `IsNot…()` pair for the booleans.

| Property             | Expectation                                |
|----------------------|--------------------------------------------|
| `Comments`           | `HasComments(string)`                      |
| `CompanyName`        | `HasCompanyName(string)`                   |
| `FileDescription`    | `HasFileDescription(string)`               |
| `FileName`           | `HasFileName(string)`                      |
| `FileVersion`        | `HasFileVersion(string)`                   |
| `InternalName`       | `HasInternalName(string)`                  |
| `Language`           | `HasLanguage(string)`                      |
| `LegalCopyright`     | `HasLegalCopyright(string)`                |
| `LegalTrademarks`    | `HasLegalTrademarks(string)`               |
| `OriginalFilename`   | `HasOriginalFilename(string)`              |
| `PrivateBuild`       | `HasPrivateBuild(string)`                  |
| `ProductName`        | `HasProductName(string)`                   |
| `ProductVersion`     | `HasProductVersion(string)`                |
| `SpecialBuild`       | `HasSpecialBuild(string)`                  |
| `FileBuildPart`      | `HasFileBuildPart(int)`                    |
| `FileMajorPart`      | `HasFileMajorPart(int)`                    |
| `FileMinorPart`      | `HasFileMinorPart(int)`                    |
| `FilePrivatePart`    | `HasFilePrivatePart(int)`                  |
| `ProductBuildPart`   | `HasProductBuildPart(int)`                 |
| `ProductMajorPart`   | `HasProductMajorPart(int)`                 |
| `ProductMinorPart`   | `HasProductMinorPart(int)`                 |
| `ProductPrivatePart` | `HasProductPrivatePart(int)`               |
| `IsDebug`            | `IsDebug()` / `IsNotDebug()`               |
| `IsPatched`          | `IsPatched()` / `IsNotPatched()`           |
| `IsPreRelease`       | `IsPreRelease()` / `IsNotPreRelease()`     |
| `IsPrivateBuild`     | `IsPrivateBuild()` / `IsNotPrivateBuild()` |
| `IsSpecialBuild`     | `IsSpecialBuild()` / `IsNotSpecialBuild()` |
