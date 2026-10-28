using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed class IsNotReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFileIsNotReadOnly_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).IsNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFileIsReadOnly_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetAttributes(path, FileAttributes.ReadOnly);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).IsNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             is not read-only,
					             but it was
					             """);
			}
		}
	}
}
