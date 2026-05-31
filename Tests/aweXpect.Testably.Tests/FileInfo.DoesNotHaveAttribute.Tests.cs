using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class DoesNotHaveAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAttributeIsAbsent_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotHaveAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAttributeIsPresent_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetAttributes(path, FileAttributes.ReadOnly);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotHaveAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             does not have attribute ReadOnly,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotHaveAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotHaveAttribute(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The unexpected file attributes must include at least one flag.*").AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
