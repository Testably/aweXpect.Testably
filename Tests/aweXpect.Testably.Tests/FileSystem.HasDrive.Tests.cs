#if NET8_0_OR_GREATER
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDrive
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveExists_ShouldSucceed()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				sut.WithDrive("D:");

				async Task Act()
				{
					await That(sut).HasDrive("D:\\");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveNameDiffersInCase_ShouldSucceed()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				sut.WithDrive("D:");

				async Task Act()
				{
					await That(sut).HasDrive("d:\\");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveNameMissesTrailingSeparator_ShouldSucceed()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				sut.WithDrive("D:");

				async Task Act()
				{
					await That(sut).HasDrive("D:");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveIsMissing_ShouldFail()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));

				async Task Act()
				{
					await That(sut).HasDrive("Z:\\");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has drive 'Z:\',
					             but it did not exist
					             """);
			}
		}
	}
}

#endif
