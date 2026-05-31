#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class DoesNotHaveVolumeLabel
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenVolumeLabelMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("C:");
				string actualLabel = driveInfo.VolumeLabel;

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasVolumeLabel(actualLabel));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that driveInfo
					              does not have volume label not equal to "{actualLabel}",
					              but it did
					              """);
			}
		}
	}
}

#endif
