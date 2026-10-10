using aweXpect.Customization;
using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed class WithLastAccessTime
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
					fileSystem.File.SetLastAccessTimeUtc(path, actual);

					async Task Act()
					{
						await That(fileSystem).HasFile(path).WithLastAccessTime(expected);
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
					fileSystem.File.SetLastAccessTimeUtc(path, actual);

					async Task Act()
					{
						await That(fileSystem).HasFile(path).WithLastAccessTime(expected);
					}

					using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.FromSeconds(2)))
					{
						await That(Act).Throws()
							.WithMessage($"""
							              Expected that fileSystem
							              has file '{path}' with last access time equal to {Formatter.Format(expected)} ± 0:02,
							              but it was {Formatter.Format(actual)}
							              """)
							.Because("the message shows the customized default tolerance that was applied");
					}
				}

				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
						              but it did not exist
						              """)
						.Because("a missing file has no last access time to compare");
				}

				[Fact]
				public async Task WhenLastAccessTimeDiffers_WithLocalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenLastAccessTimeDiffers_WithUniversalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenLastAccessTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastAccessTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastAccessTimeIsUnspecified_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = new(2020, 2, 1, 12, 0, 0, DateTimeKind.Unspecified);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastAccessTimeMatches_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastAccessTimeMatches_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTimeUtc(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile("foo.txt").WithLastAccessTime(expectedTime));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNegated_WhenLastAccessTimeMatches_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastAccessTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithLastAccessTime(expectedTime));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
						              but it did and was {Formatter.Format(expectedTime)}
						              """)
						.Because("the negation belongs to the verb only");
				}

				[Fact]
				public async Task WhenNegated_WhenPathIsADirectory_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo";
					sut.Directory.CreateDirectory(path);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithLastAccessTime(expectedTime));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPathIsADirectory_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo";
					sut.Directory.CreateDirectory(path);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastAccessTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last access time equal to {Formatter.Format(expectedTime)},
						              but it was a directory
						              """)
						.Because("the last access time of a directory is not the last access time of a file");
				}
			}
		}
	}
}
