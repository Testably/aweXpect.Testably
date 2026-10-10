using System.IO.Abstractions;
using aweXpect.Customization;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasCreationTime
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDifferenceIsWithinDefaultTolerance_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected);
				}

				using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.FromSeconds(2)))
				{
					await That(Act).DoesNotThrow()
						.Because("the customized default tolerance applies when no tolerance is specified");
				}
			}

			[Fact]
			public async Task WhenDifferenceExceedsDefaultTolerance_ShouldFailAndIncludeToleranceInMessage()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(3);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected);
				}

				using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.FromSeconds(2)))
				{
					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              has creation time equal to {Formatter.Format(expected)} ± 0:02,
						              but it was {Formatter.Format(actual)}
						              """)
						.Because("the message shows the customized default tolerance that was applied");
				}
			}

			[Fact]
			public async Task WhenCreationTimeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that fileInfo
					              has creation time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenCreationTimeIsUnspecified_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = new(2020, 2, 1, 12, 0, 0, DateTimeKind.Unspecified);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTime(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCreationTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithinTolerance_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasCreationTime(expected).Within(TimeSpan.FromSeconds(2));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndCreationTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetCreationTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasCreationTime(expected));
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that fileInfo
					              does not have creation time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
