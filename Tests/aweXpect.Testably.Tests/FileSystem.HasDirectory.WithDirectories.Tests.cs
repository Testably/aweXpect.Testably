using System.IO;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class WithDirectories
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenItemCountDiffers_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1")
						.WithSubdirectory("directory2"));

					async Task Act()
					{
						await That(sut).HasDirectory(path).WithDirectories(f => f.HasCount().EqualTo(3));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories have exactly 3 items,
						              but subdirectories had only 2 items

						              Collection (subdirectories):
						              [
						                *,
						                *
						              ]
						              """).AsWildcard();
				}

				[Fact]
				public async Task AllAreEmpty_WhenSubdirectoryIsNotEmpty_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1").Initialized(s => s
							.WithFile("bar.txt")));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.IsEmpty()));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all are empty,
						              but none of at least 1 were

						              Not matching items (subdirectories):
						              [
						                foo{Path.DirectorySeparatorChar}directory1,
						                (… and maybe more)
						              ]

						              Collection (subdirectories):
						              [
						                foo{Path.DirectorySeparatorChar}directory1,
						                (… and maybe more)
						              ]
						              """).IgnoringNewlineStyle();
				}

				[Fact]
				public async Task AllAreEmpty_WhenSubdirectoriesAreEmpty_ShouldSucceed()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1")
						.WithSubdirectory("directory2"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.IsEmpty()));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task AllAreNotEmpty_WhenSubdirectoryIsEmpty_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.IsNotEmpty()));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all are not empty,
						              but none of at least 1 were

						              Not matching items (subdirectories):
						              [
						                foo{Path.DirectorySeparatorChar}directory1,
						                (… and maybe more)
						              ]

						              Collection (subdirectories):
						              [
						                foo{Path.DirectorySeparatorChar}directory1,
						                (… and maybe more)
						              ]
						              """).IgnoringNewlineStyle();
				}

				[Fact]
				public async Task AllHaveDirectory_WhenDirectoryIsMissing_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.HasDirectory("bar")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all have directory "bar",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural subdirectories");
				}

				[Fact]
				public async Task AllHaveFile_WhenFileIsMissing_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.HasFile("bar.txt").WithContent("baz")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all have file "bar.txt" with content equal to "baz",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural subdirectories");
				}

				[Fact]
				public async Task AllHaveFile_WhenNegated_WhenFileHasTheContent_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1").Initialized(s => s
							.WithFile("bar.txt").Which(f => f.HasStringContent("baz"))));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir
								=> dir.DoesNotComplyWith(it => it.HasFile("bar.txt").WithContent("baz"))));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all do not have file "bar.txt" with content equal to "baz",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the negation belongs to the plural verb only");
				}

				[Fact]
				public async Task AllHaveName_WhenNameDiffers_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1"));

					async Task Act()
					{
						await That(sut).HasDirectory(path)
							.WithDirectories(dirs => dirs.All().ComplyWith(dir => dir.HasName("bar")));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has directory {Formatter.Format(path)} whose subdirectories all have name equal to "bar",
						              but none of at least 1 did*
						              """).AsWildcard()
						.Because("the verb agrees with the plural subdirectories");
				}

				[Fact]
				public async Task WhenItemCountMatches_ShouldSucceed()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory(path).Initialized(d => d
						.WithSubdirectory("directory1")
						.WithSubdirectory("directory2"));

					async Task Act()
					{
						await That(sut).HasDirectory(path).WithDirectories(f => f.HasCount().EqualTo(2));
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
