#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class DoesNotHaveDriveFormat
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveFormatMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetDriveFormat("NTFS"));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasDriveFormat("NTFS"));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             does not have drive format not equal to "NTFS",
					             but it did
					             """);
			}
		}
	}
}

#endif
