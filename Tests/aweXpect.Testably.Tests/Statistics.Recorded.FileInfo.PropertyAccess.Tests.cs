using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileInfoPropertyAccess
		{
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded no get of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 1 time
					             """);
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded no set of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 1 time
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenNegatingNeverGetAndNotAccessed_ShouldFailWithAtLeastOneGetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Get().Never());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded at least one get of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 0 times
					             """);
			}

			[Fact]
			public async Task WhenNegatingNeverSetAndNotAccessed_ShouldFailWithAtLeastOneSetWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().FileInfo["foo.txt"].IsReadOnly.Set().Never());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded at least one set of FileInfo["foo.txt"].IsReadOnly,
					             but it was recorded 0 times
					             """);
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             did not record a get of FileInfo["foo.txt"].IsReadOnly exactly once,
					             but it was recorded 1 time
					             """);
			}
		}
	}
}
