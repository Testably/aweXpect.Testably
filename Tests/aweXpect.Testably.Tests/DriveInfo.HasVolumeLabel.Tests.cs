#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class HasVolumeLabel
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenVolumeLabelMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("C:");
				string actualLabel = driveInfo.VolumeLabel;

				async Task Act()
				{
					await That(driveInfo).HasVolumeLabel(actualLabel);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenVolumeLabelDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("C:");

				async Task Act()
				{
					await That(driveInfo).HasVolumeLabel("definitely-not-the-label");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             has volume label equal to "definitely-not-the-label",
					             but it was *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedIfDiffers()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("C:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasVolumeLabel("definitely-not-the-label"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
