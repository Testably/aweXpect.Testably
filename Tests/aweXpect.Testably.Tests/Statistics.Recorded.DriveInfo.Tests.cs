using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class DriveInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task GetDrives_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DriveInfo.GetDrives();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().DriveInfo.GetDrives().Once();
					}

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task New_WithDriveNameFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:");
					_ = fileSystem.DriveInfo.New("D:");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo.New(n => n == "C:").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task New_WithDriveNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo.New(n => n == "C:").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to DriveInfo.New with driveName matching n => n == "C:" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Wrap_WithDriveInfoFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo.Wrap(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to DriveInfo.Wrap with driveInfo matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif
			}

#if NET8_0_OR_GREATER
			public sealed class NameTests
			{
				[Fact]
				public async Task Name_Get_ShouldRecordPerDrive()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").Name;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].Name.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PropertyTests
			{
				[Fact]
				public async Task AvailableFreeSpace_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").AvailableFreeSpace;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].AvailableFreeSpace.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DriveFormat_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").DriveFormat;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].DriveFormat.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DriveType_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").DriveType;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].DriveType.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IsReady_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").IsReady;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].IsReady.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task RootDirectory_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").RootDirectory;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].RootDirectory.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task TotalFreeSpace_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").TotalFreeSpace;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].TotalFreeSpace.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task TotalSize_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").TotalSize;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].TotalSize.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task VolumeLabel_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
					_ = fileSystem.DriveInfo.New("C:").VolumeLabel;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].VolumeLabel.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DriveInfo["C:"].DriveFormat.Get().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a get of DriveInfo["C:"].DriveFormat exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
#endif
		}
	}
}
