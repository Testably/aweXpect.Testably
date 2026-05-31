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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory '{path}' whose subdirectories has exactly 3 items,
						              but found only 2

						              Collection:
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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              has directory '{path}' whose subdirectories is empty for all items,
						              but not all were

						              Not matching items:
						              [
						                foo{Path.DirectorySeparatorChar}directory1,
						                (… and maybe others)
						              ]

						              Collection:
						              [
						                foo{Path.DirectorySeparatorChar}directory1
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
