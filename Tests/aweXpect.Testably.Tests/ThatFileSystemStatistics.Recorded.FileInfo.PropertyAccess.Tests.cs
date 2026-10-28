using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileSystemStatistics
{
	public sealed partial class Recorded
	{
		public sealed class FileInfoPropertyAccess
		{
			[Test]
			public async Task WhenAccessed_NeverGet_ShouldFailWithNoGetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
				_ = fileInfo.IsReadOnly;

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.FileInfo["foo.txt"].IsReadOnly.Get().Never();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded no get of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 1 time
					             """);
			}

			[Test]
			public async Task WhenNegatingExactlyGetAndAccessed_ShouldFailWithDidNotRecordGetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
				_ = fileInfo.IsReadOnly;

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Get().Once());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has not recorded a get of FileInfo["foo.txt"].IsReadOnly exactly once,
					             but it was recorded 1 time
					             """);
			}

			[Test]
			public async Task WhenNegatingNeverGetAndAccessed_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
				_ = fileInfo.IsReadOnly;

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Get().Never());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatingNeverGetAndNotAccessed_ShouldFailWithAtLeastOneGetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Get().Never());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded at least one get of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 0 times
					             """);
			}

			[Test]
			public async Task WhenNegatingNeverSetAndNotAccessed_ShouldFailWithAtLeastOneSetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Set().Never());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded at least one set of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 0 times
					             """);
			}

			[Test]
			public async Task WhenNeverAccessed_NeverGet_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.FileInfo["foo.txt"].IsReadOnly.Get().Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSet_NeverSet_ShouldFailWithNoSetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
				fileInfo.IsReadOnly = false;

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.FileInfo["foo.txt"].IsReadOnly.Set().Never();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded no set of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 1 time
					             """);
			}
		}
	}
}
