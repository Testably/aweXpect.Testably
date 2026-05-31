using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class HasAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAttributeIsAbsent_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).HasAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that dirInfo
					             has attribute ReadOnly,
					             but it was Directory
					             """);
			}

			[Fact]
			public async Task WhenAttributeIsPresent_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).HasAttribute(FileAttributes.Directory);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).HasAttribute(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected file attributes must include at least one flag.*").AsWildcard().And
					.WithParamName("expected");
			}
		}
	}
}
