using Testably.Abstractions.Testing;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed class WithLastWriteTime
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
						              but it did not exist
						              """)
						.Because("a missing file has no last write time to compare");
				}

				[Fact]
				public async Task WhenLastWriteTimeDiffers_WithLocalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenLastWriteTimeDiffers_WithUniversalTime_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
						              but it was {Formatter.Format(actualTime)}
						              """);
				}

				[Fact]
				public async Task WhenLastWriteTimeDiffersWithinTolerance_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTime(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastWriteTimeDiffersWithinTolerance_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					DateTime actualTime = expectedTime.AddSeconds(1);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTimeUtc(path, actualTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime)
							.Within(TimeSpan.FromSeconds(2));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastWriteTimeIsUnspecified_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = new(2020, 2, 1, 12, 0, 0, DateTimeKind.Unspecified);
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastWriteTimeMatches_WithLocalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenLastWriteTimeMatches_WithUniversalTime_ShouldSucceed()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToUniversalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTimeUtc(path, expectedTime);

					async Task Act()
					{
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
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
						await That(sut).DoesNotComplyWith(it => it.HasFile("foo.txt").WithLastWriteTime(expectedTime));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNegated_WhenLastWriteTimeMatches_ShouldFail()
				{
					MockFileSystem sut = new();
					DateTime expectedTime = CurrentTime().ToLocalTime();
					string path = "foo.txt";
					sut.File.WriteAllText(path, "");
					sut.File.SetLastWriteTime(path, expectedTime);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithLastWriteTime(expectedTime));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
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
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithLastWriteTime(expectedTime));
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
						await That(sut).HasFile(path).WithLastWriteTime(expectedTime);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with last write time equal to {Formatter.Format(expectedTime)},
						              but it was a directory
						              """)
						.Because("the last write time of a directory is not the last write time of a file");
				}
			}
		}
	}
}
