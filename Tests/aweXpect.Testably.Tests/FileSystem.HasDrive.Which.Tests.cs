#if NET8_0_OR_GREATER
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasDrive
	{
		public sealed class Which
		{
			public sealed class Tests
			{
				[Fact]
				public async Task IsReady_WhenDriveIsReady_ShouldSucceed()
				{
					MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					sut.WithDrive("D:");

					async Task Act()
					{
						await That(sut).HasDrive("D:\\").Which.IsReady();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task HasTotalSize_WhenSizeDiffers_ShouldFail()
				{
					MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					sut.WithDrive("D:", d => d.SetTotalSize(2048));

					async Task Act()
					{
						await That(sut).HasDrive("D:\\").Which.HasTotalSize(1024);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that sut
						             has drive 'D:\' which has total size 1024,
						             but it was 2048
						             """);
				}

				[Fact]
				public async Task WhenDriveIsMissing_ChainedExpectation_ShouldFailWithoutNullReference()
				{
					MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));

					async Task Act()
					{
						await That(sut).HasDrive("Z:\\").Which.HasTotalSize(1024);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that sut
						             has drive 'Z:\' which has total size 1024,
						             but it did not exist
						             """);
				}

				[Fact]
				public async Task ChainedAnd_HasTotalSize_And_IsReady_ShouldSucceed()
				{
					MockFileSystem sut = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					sut.WithDrive("D:", d => d.SetTotalSize(4096));

					async Task Act()
					{
						await That(sut).HasDrive("D:\\")
							.Which.HasTotalSize(4096).And.IsReady();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}

#endif
