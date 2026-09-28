using System.IO;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasOldName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenOldNameDiffers_ShouldFail()
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
					await That(renamed!).HasOldName("other-name.txt");
				}

				await That(Act).Throws()
					.WithMessage("*has old name equal to \"other-name.txt\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldNameIsNull_AndExpectedIsNotNull_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasOldName("foo.txt");
				}

				await That(Act).Throws()
					.WithMessage("*has old name equal to \"foo.txt\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenOldNameMatches_ShouldSucceed()
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
					await That(renamed!).HasOldName(renamed!.OldName!);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
