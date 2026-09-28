#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class IsReady
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveIsReady_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetIsReady(true));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).IsReady();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDriveIsNotReady_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetIsReady(false));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).IsReady();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that driveInfo
					             is ready,
					             but it was not
					             """);
			}
		}
	}
}

#endif
