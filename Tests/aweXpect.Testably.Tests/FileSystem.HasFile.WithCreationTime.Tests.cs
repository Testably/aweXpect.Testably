using aweXpect.Customization;
using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed class WithCreationTime
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

					async Task Act()
					{
						await That(fileSystem).HasFile(path).WithCreationTime(expected);
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

					async Task Act()
					{
						await That(fileSystem).HasFile(path).WithCreationTime(expected);
					}

					using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.FromSeconds(2)))
					{
						await That(Act).Throws()
							.WithMessage($"""
							              Expected that fileSystem
							              has file '{path}' with creation time equal to {Formatter.Format(expected)} ± 0:02,
							              but it was {Formatter.Format(actual)}
							              """)
							.Because("the message shows the customized default tolerance that was applied");
					}
				}

				[Fact]
				public async Task WhenCreationTimeDiffers_WithLocalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenCreationTimeDiffers_WithUniversalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenCreationTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCreationTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
						              but it did not exist
						              """)
						.Because("a missing file has no creation time to compare");
				}

				[Fact]
				public async Task WhenFileDoesNotExist_WithTolerance_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with creation time equal to {Formatter.Format(expectedTime)} ± 0:02,
						              but it did not exist
						              """)
						.Because("a missing file fails regardless of the tolerance");
				}

				[Fact]
				public async Task WhenNegated_WhenCreationTimeMatches_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithCreationTime(expectedTime));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
						              but it did and was {Formatter.Format(expectedTime)}
						              """);
				}

				[Fact]
				public async Task WhenNegated_WhenCreationTimeDiffers_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, actualTime);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithCreationTime(expectedTime));
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
						await That(sut).DoesNotComplyWith(it => it.HasFile("foo.txt").WithCreationTime(expectedTime));
					}

					await That(Act).DoesNotThrow();
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
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithCreationTime(expectedTime));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCreationTimeIsUnspecified_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = new(2020, 2, 1, 12, 0, 0, DateTimeKind.Unspecified);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}


				[Fact]
				public async Task WhenCreationTimeMatches_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCreationTimeMatches_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetCreationTimeUtc(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
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
						await That(sut).HasFile(path).WithCreationTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with creation time equal to {Formatter.Format(expectedTime)},
						              but it was a directory
						              """)
						.Because("the creation time of a directory is not the creation time of a file");
				}
			}
		}
	}
}
