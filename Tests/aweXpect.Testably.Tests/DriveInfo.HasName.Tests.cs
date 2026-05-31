#if NET8_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DriveInfo
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:");
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasName(driveInfo.Name);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:");
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).HasName("Z:\\");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that driveInfo
					             has name equal to "Z:\",
					             but it was "D:\" which differs at index 0:
					                ↓ (actual)
					               "D:\"
					               "Z:\"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenNegated_ShouldSucceedIfDiffers()
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:");
				IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

				async Task Act()
				{
					await That(driveInfo).DoesNotComplyWith(d => d.HasName("Z:\\"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
