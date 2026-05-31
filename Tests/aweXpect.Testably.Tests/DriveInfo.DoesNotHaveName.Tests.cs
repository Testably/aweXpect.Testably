#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class DoesNotHaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:");
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasName(driveInfo.Name));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             does not have name not equal to "D:\",
					             but it did
					             """);
			}
		}
	}
}

#endif
