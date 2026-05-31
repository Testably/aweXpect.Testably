using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class DoesNotHaveAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAttributeIsAbsent_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAttributeIsPresent_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveAttribute(FileAttributes.Directory);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that dirInfo
					             does not have attribute Directory,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveAttribute(FileAttributes.Directory);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				MockFileSystem fileSystem = new();
				string path = "foo";
				fileSystem.Directory.CreateDirectory(path);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New(path);

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveAttribute(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The unexpected file attributes must include at least one flag.*").AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
