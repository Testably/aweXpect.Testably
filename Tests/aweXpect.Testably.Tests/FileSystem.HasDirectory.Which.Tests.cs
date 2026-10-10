using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class Which
		{
			public sealed class Tests
			{
				[Fact]
				public async Task HasFile_WhenFileExists_ShouldSucceed()
				{
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("logs").Initialized(d => d
						.WithFile("today.log"));

					async Task Act()
					{
						await That(sut).HasDirectory("logs").Which.HasFile("today.log");
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsEmpty_WhenEmpty_ShouldSucceed()
				{
					MockFileSystem sut = new();
					sut.Directory.CreateDirectory("logs");

					async Task Act()
					{
						await That(sut).HasDirectory("logs").Which.IsEmpty();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsEmpty_WhenNotEmpty_ShouldFail()
				{
					MockFileSystem sut = new();
					sut.Initialize().WithSubdirectory("logs").Initialized(d => d
						.WithFile("today.log"));

					async Task Act()
					{
						await That(sut).HasDirectory("logs").Which.IsEmpty();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that sut
						             has directory "logs" which is empty,
						             but the directory was not
						             """);
				}
			}
		}
	}
}
