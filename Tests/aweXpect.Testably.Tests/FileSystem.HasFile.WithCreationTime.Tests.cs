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

					await That(Act).ThrowsException()
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

					await That(Act).ThrowsException()
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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with creation time not equal to {Formatter.Format(expectedTime)},
						              but it did and it was {Formatter.Format(expectedTime)}
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
			}
		}
	}
}
