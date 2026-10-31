# File system

Describes the possible expectations for the files, directories and drives of a `MockFileSystem`, either through the
`IFileSystem` or directly on an `IFileInfo`, `IDirectoryInfo`, `IDriveInfo` or `IFileVersionInfo`.

Expectations for [`IFileSystem`](#files-directories-and-drives):

| Expectation                                     | Negated                | Summary                                                              |
|-------------------------------------------------|------------------------|----------------------------------------------------------------------|
| [`HasFile`](#files-directories-and-drives)      | `DoesNotHaveFile`      | contains the file, optionally with its content and timestamps        |
| [`HasDirectory`](#files-directories-and-drives) | `DoesNotHaveDirectory` | contains the directory, optionally with its files and subdirectories |
| [`HasDrive`](#files-directories-and-drives)     | `DoesNotHaveDrive`     | contains the drive                                                   |

Expectations for [`IFileInfo`](#file):

| Expectation                  | Negated                   | Summary                                                          |
|------------------------------|---------------------------|------------------------------------------------------------------|
| [`Exists`](#file)            | `DoesNotExist`            | the file exists                                                  |
| [`HasName`](#file)           |                           | has the expected name                                            |
| [`HasExtension`](#file)      |                           | has the expected extension                                       |
| [`HasLength`](#file)         |                           | has the expected length                                          |
| [`HasContent`](#file)        | `HasContent().NotEqualTo` | has the expected string or binary content                        |
| [`IsReadOnly`](#file)        | `IsNotReadOnly`           | the file is read-only                                            |
| [`HasAttribute`](#file)      | `DoesNotHaveAttribute`    | has the attribute flag                                           |
| [`HasCreationTime`](#file)   |                           | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](#file) |                           | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](#file)  |                           | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](#file)       |                           | the containing directory meets expectations (.NET 10 or later)   |

Expectations for [`IDirectoryInfo`](#directory):

| Expectation                       | Negated                | Summary                                                          |
|-----------------------------------|------------------------|------------------------------------------------------------------|
| [`Exists`](#directory)            | `DoesNotExist`         | the directory exists                                             |
| [`HasName`](#directory)           |                        | has the expected name                                            |
| [`IsEmpty`](#directory)           | `IsNotEmpty`           | contains no files or subdirectories                              |
| [`HasFile`](#directory)           | `DoesNotHaveFile`      | contains the file                                                |
| [`HasDirectory`](#directory)      | `DoesNotHaveDirectory` | contains the subdirectory                                        |
| [`HasAttribute`](#directory)      | `DoesNotHaveAttribute` | has the attribute flag                                           |
| [`HasCreationTime`](#directory)   |                        | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](#directory) |                        | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](#directory)  |                        | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](#directory)       |                        | the parent directory meets expectations (.NET 10 or later)       |

Expectations for [`IDriveInfo`](#drive):

| Expectation                       | Negated      | Summary                               |
|-----------------------------------|--------------|---------------------------------------|
| [`IsReady`](#drive)               | `IsNotReady` | the drive is ready                    |
| [`HasName`](#drive)               |              | has the expected name                 |
| [`HasVolumeLabel`](#drive)        |              | has the expected volume label         |
| [`HasDriveFormat`](#drive)        |              | has the expected drive format         |
| [`HasDriveType`](#drive)          |              | has the expected drive type           |
| [`HasTotalSize`](#drive)          |              | has the expected total size           |
| [`HasTotalFreeSpace`](#drive)     |              | has the expected total free space     |
| [`HasAvailableFreeSpace`](#drive) |              | has the expected available free space |

Expectations for [`IFileVersionInfo`](#file-version-info):

| Expectation                                                              | Negated         | Summary                      |
|--------------------------------------------------------------------------|-----------------|------------------------------|
| [`HasCompanyName`, `HasFileMajorPart`, `IsDebug`, …](#file-version-info) | `IsNotDebug`, … | one expectation per property |

The samples on this page use the following file system:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.Directory.CreateDirectory("beatles/revolver");
fileSystem.File.WriteAllText("beatles/abbey-road/something.txt", "something");
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");
fileSystem.File.WriteAllText("let-it-be-remastered.txt", "LET IT BE");
fileSystem.File.WriteAllText("hey-jude.txt", "hey jude");
fileSystem.File.SetAttributes("hey-jude.txt", FileAttributes.ReadOnly);
```

## Files, directories and drives

You can verify that a file, directory or drive exists in an `IFileSystem`, or that it does not:

```csharp
await Expect.That(fileSystem).HasDirectory("beatles/abbey-road");
await Expect.That(fileSystem).HasFile("let-it-be.txt");
await Expect.That(fileSystem).HasDrive("D:\\");

await Expect.That(fileSystem).DoesNotHaveDirectory("beatles/white-album");
await Expect.That(fileSystem).DoesNotHaveFile("yesterday.txt");
await Expect.That(fileSystem).DoesNotHaveDrive("E:\\");
```

For example, `Expect.That(fileSystem).HasFile("yesterday.txt")` fails with:

```text title="Failure message"
Expected that fileSystem
has file "yesterday.txt",
but it did not exist
```

`HasDrive` matches the drive name case-insensitively against `IFileSystem.DriveInfo.GetDrives()`. UNC drives are not
returned by `GetDrives()` and are therefore not supported by `HasDrive`.

### File content and timestamps

You can verify the content of the file found by `HasFile(path)`:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotEqualTo("let it go");
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent(Encoding.UTF8.GetBytes("let it be"));
await Expect.That(fileSystem).HasFile("let-it-be.txt").WhoseContent(c => c.StartsWith("let it"));
```

The string content supports the same options as
[comparing strings](https://docs.testably.org/aweXpect/values/string#equality), and `WhoseContent` accepts any
[string expectation](https://docs.testably.org/aweXpect/values/string). When the content differs, the failure message
shows where and includes the file content:

```text title="Failure message"
Expected that fileSystem
has file "let-it-be.txt" with content equal to "let it go",
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
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotSameAs("hey-jude.txt");
```

You can verify the timestamps of the file, and widen the comparison to a window with `.Within(tolerance)`:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithCreationTime(DateTime.Now).Within(1.Seconds());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastAccessTime(DateTime.Now).Within(1.Seconds());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastWriteTime(DateTime.Now).Within(1.Seconds());
```

### Directory content

You can verify the files and subdirectories of the directory found by `HasDirectory(path)`:

```csharp
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
IFileInfo readOnlyFileInfo = fileSystem.FileInfo.New("hey-jude.txt");

await Expect.That(fileInfo).Exists();
await Expect.That(fileSystem.FileInfo.New("yesterday.txt")).DoesNotExist();

await Expect.That(fileInfo).HasName("let-it-be.txt");
await Expect.That(fileInfo).HasExtension(".txt");
await Expect.That(fileInfo).HasLength(9);

await Expect.That(readOnlyFileInfo).IsReadOnly();
await Expect.That(fileInfo).IsNotReadOnly();

await Expect.That(readOnlyFileInfo).HasAttribute(FileAttributes.ReadOnly);
await Expect.That(fileInfo).DoesNotHaveAttribute(FileAttributes.ReadOnly);

await Expect.That(fileInfo).HasCreationTime(DateTime.Now).Within(1.Seconds());
await Expect.That(fileInfo).HasLastAccessTime(DateTime.Now).Within(1.Seconds());
await Expect.That(fileInfo).HasLastWriteTime(DateTime.Now).Within(1.Seconds());
```

For example, `Expect.That(fileInfo).HasLength(10)` fails with:

```text title="Failure message"
Expected that fileInfo
has length 10,
but it was 9
```

You can verify the content of the file, or compare it with another file on the same file system, like with
[`HasFile`](#file-content-and-timestamps):

```csharp
await Expect.That(fileInfo).HasContent("let it be");
await Expect.That(fileInfo).HasContent(Encoding.UTF8.GetBytes("let it be"));
await Expect.That(fileInfo).HasContent().NotEqualTo("let it go");
await Expect.That(fileInfo).HasContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
await Expect.That(fileInfo).HasContent().NotSameAs("hey-jude.txt");
```

`HasAttribute` and `DoesNotHaveAttribute` check whether the flag is contained, so
`FileAttributes.ReadOnly | FileAttributes.Hidden` satisfies `HasAttribute(FileAttributes.ReadOnly)`. The empty
(`default`) value throws an `ArgumentException`, because it would be contained in every value.

:::note[.NET 10 or later]
`WhoseParent` continues with the containing directory, so that the [directory](#directory) expectations can be used:

```csharp
IFileInfo something = fileSystem.FileInfo.New("beatles/abbey-road/something.txt");

await Expect.That(something).WhoseParent.HasName("abbey-road").And.IsNotEmpty();
```
:::

## Directory

You can verify the properties of an `IDirectoryInfo`:

```csharp
IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("beatles");
IDirectoryInfo emptyDirInfo = fileSystem.DirectoryInfo.New("beatles/revolver");

await Expect.That(dirInfo).Exists();
await Expect.That(fileSystem.DirectoryInfo.New("beatles/white-album")).DoesNotExist();

await Expect.That(dirInfo).HasName("beatles");

await Expect.That(emptyDirInfo).IsEmpty();
await Expect.That(dirInfo).IsNotEmpty();

await Expect.That(dirInfo).HasFile("abbey-road/something.txt");
await Expect.That(dirInfo).DoesNotHaveFile("abbey-road/yesterday.txt");
await Expect.That(dirInfo).HasDirectory("abbey-road").Which.HasFile("something.txt");
await Expect.That(dirInfo).DoesNotHaveDirectory("white-album");

await Expect.That(dirInfo).HasAttribute(FileAttributes.Directory);
await Expect.That(dirInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await Expect.That(dirInfo).HasCreationTime(DateTime.Now).Within(1.Seconds());
await Expect.That(dirInfo).HasLastAccessTime(DateTime.Now).Within(1.Seconds());
await Expect.That(dirInfo).HasLastWriteTime(DateTime.Now).Within(1.Seconds());
```

`HasFile` and `HasDirectory` resolve the path relative to the directory and continue like on the
[file system](#files-directories-and-drives), so `.WithContent(…)`, `.WithLastWriteTime(…)`, `.Which` etc. work as
well.

:::note[.NET 10 or later]
`WhoseParent` continues with the parent directory:

```csharp
await Expect.That(emptyDirInfo).WhoseParent.HasName("beatles");
```
:::

## Drive

You can verify the properties of an `IDriveInfo`:

```csharp
IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

await Expect.That(driveInfo).HasAvailableFreeSpace(2048);
await Expect.That(driveInfo).HasTotalSize(2048).And.HasTotalFreeSpace(2048);
await Expect.That(driveInfo).HasDriveFormat("NTFS");
await Expect.That(driveInfo).HasDriveType(DriveType.Fixed);
await Expect.That(driveInfo).HasName(driveInfo.Name).And.HasVolumeLabel(driveInfo.VolumeLabel);
await Expect.That(driveInfo).IsReady();
```

## File version info

You can verify an `IFileVersionInfo` obtained via `MockFileSystem.FileVersionInfo.GetVersionInfo`. Its values come
from `MockFileSystem.WithFileVersionInfo(glob, builder)`:

```csharp
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

<details>
<summary>Expectations per property</summary>

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

</details>
