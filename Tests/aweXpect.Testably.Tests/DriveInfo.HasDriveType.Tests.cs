#if NET8_0_OR_GREATER
using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class HasDriveType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveTypeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveType(DriveType.Fixed));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasDriveType(DriveType.Fixed);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveTypeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveType(DriveType.Fixed));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasDriveType(DriveType.Network);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that driveInfo
					             has drive type Network,
					             but it was Fixed
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedIfDiffers()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveType(DriveType.Fixed));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasDriveType(DriveType.Network));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
