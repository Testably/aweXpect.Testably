using System.IO;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasOldPath
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenOldPathDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				ChangeDescription? renamed = null;
				using IAwaitableCallback<ChangeDescription> registration =
					fileSystem.Notify.OnEvent(c =>
					{
						if (c.ChangeType == WatcherChangeTypes.Renamed)
						{
							renamed ??= c;
						}
					});
				fileSystem.File.Move("foo.txt", "bar.txt");

				async Task Act()
				{
					await That(renamed!).HasOldPath("totally-different-path");
				}

				await That(Act).Throws()
					.WithMessage("*has old path equal to \"totally-different-path\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldPathIsNull_AndExpectedIsNotNull_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasOldPath("foo.txt");
				}

				await That(Act).Throws()
					.WithMessage("*has old path equal to \"foo.txt\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldPathMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				ChangeDescription? renamed = null;
				using IAwaitableCallback<ChangeDescription> registration =
					fileSystem.Notify.OnEvent(c =>
					{
						if (c.ChangeType == WatcherChangeTypes.Renamed)
						{
							renamed ??= c;
						}
					});
				fileSystem.File.Move("foo.txt", "bar.txt");

				async Task Act()
				{
					await That(renamed!).HasOldPath(renamed!.OldPath!);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
