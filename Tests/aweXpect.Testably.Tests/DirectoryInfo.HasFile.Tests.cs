using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class HasFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ChainedWhich_HasLength_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt").Which(f => f.HasStringContent("abc")));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasFile("bar.txt").Which.HasLength(3);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ChainedWhich_HasLength_WhenFails_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt").Which(f => f.HasStringContent("abc")));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasFile("bar.txt").Which.HasLength(99);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             has file "bar.txt" which has length 99,
					             but the file was 3
					             """);
			}

			[Fact]
			public async Task WhenFileExists_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasFile("bar.txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileIsMissing_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasFile("bar.txt");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             has file "bar.txt",
					             but it did not exist
					             """);
			}

			[Fact]
			public async Task WithCreationTime_WhenFileIsMissing_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");
				DateTime expectedTime = CurrentTime().ToLocalTime();

				async Task Act()
				{
					await That(dirInfo).HasFile("bar.txt").WithCreationTime(expectedTime);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that dirInfo
					              has file "bar.txt" with creation time equal to {Formatter.Format(expectedTime)},
					              but it did not exist
					              """)
					.Because("a missing file has no creation time to compare");
			}

			[Fact]
			public async Task WithCreationTime_WhenPathIsADirectory_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo/bar");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");
				DateTime expectedTime = CurrentTime().ToLocalTime();

				async Task Act()
				{
					await That(dirInfo).HasFile("bar").WithCreationTime(expectedTime);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that dirInfo
					              has file "bar" with creation time equal to {Formatter.Format(expectedTime)},
					              but it was a directory
					              """)
					.Because("the creation time of a directory is not the creation time of a file");
			}
		}
	}
}
