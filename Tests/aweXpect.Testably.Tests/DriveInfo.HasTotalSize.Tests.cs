#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class HasTotalSize
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTotalSizeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasTotalSize(2048);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTotalSizeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasTotalSize(99);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that driveInfo
					             has total size 99,
					             but it was 2048
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedIfDiffers()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasTotalSize(99));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
