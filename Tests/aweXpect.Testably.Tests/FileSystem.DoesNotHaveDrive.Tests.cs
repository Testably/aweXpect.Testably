#if NET8_0_OR_GREATER
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class DoesNotHaveDrive
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDriveExists_ShouldFail()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				sut.WithDrive("D:");

				async Task Act()
				{
					await That(sut).DoesNotHaveDrive("D:\\");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             does not have drive 'D:\',
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenDriveIsMissing_ShouldSucceed()
			{
				MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));

				async Task Act()
				{
					await That(sut).DoesNotHaveDrive("Z:\\");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}

#endif
