using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed class HasAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAttributeIsAbsent_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             has attribute ReadOnly,
					             but it was Normal
					             """);
			}

			[Test]
			public async Task WhenAttributeIsPresent_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetAttributes(path, FileAttributes.ReadOnly);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasAttribute(FileAttributes.ReadOnly);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasAttribute(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected file attributes must include at least one flag.*").AsWildcard().And
					.WithParamName("expected");
			}
		}
	}
}
