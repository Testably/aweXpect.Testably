#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatDriveInfo
{
	public sealed class DoesNotComplyWithIsReady
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenDriveIsReady_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:", d => d.SetIsReady(true));
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.IsReady());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that driveInfo
					             is not ready,
					             but it was
					             """);
			}
		}
	}
}

#endif
