#if NET8_0_OR_GREATER
using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class DoesNotHaveDriveType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveTypeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveType(DriveType.Fixed));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasDriveType(DriveType.Fixed));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             does not have drive type Fixed,
					             but it did
					             """);
			}
		}
	}
}

#endif
