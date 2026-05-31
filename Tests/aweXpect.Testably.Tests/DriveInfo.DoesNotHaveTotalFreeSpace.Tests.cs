#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class DoesNotHaveTotalFreeSpace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTotalFreeSpaceMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasTotalFreeSpace(2048));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             does not have total free space 2048,
					             but it did
					             """);
			}
		}
	}
}

#endif
