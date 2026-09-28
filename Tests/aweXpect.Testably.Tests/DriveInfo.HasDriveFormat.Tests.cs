#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class HasDriveFormat
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveFormatMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveFormat("NTFS"));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasDriveFormat("NTFS");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveFormatDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveFormat("NTFS"));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasDriveFormat("FAT32");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that driveInfo
					             has drive format equal to "FAT32",
					             but it was "NTFS", which differs at index 0:
					                ↓ (actual)
					               "NTFS"
					               "FAT32"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedIfDiffers()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveFormat("NTFS"));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasDriveFormat("FAT32"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
