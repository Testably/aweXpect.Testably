using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed class PluralGrammar
{
	public sealed class Tests
	{
		[Fact]
		public async Task ChangeDescription_HasChangeType_ShouldUsePluralVerb()
		{
			ChangeDescription[] changes = [ChangeDescriptionTests.Capture(fs => fs.File.WriteAllText("foo.txt", "")),];

			async Task Act()
			{
				await ThatAll(changes, c => c.HasChangeType(WatcherChangeTypes.Deleted));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have change type Deleted for all items,*").AsWildcard();
		}

		[Fact]
		public async Task ChangeDescription_HasFileSystemType_ShouldUsePluralVerb()
		{
			ChangeDescription[] changes = [ChangeDescriptionTests.Capture(fs => fs.File.WriteAllText("foo.txt", "")),];

			async Task Act()
			{
				await ThatAll(changes, c => c.DoesNotHaveFileSystemType(FileSystemTypes.File));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items do not have file system type File for all items,*").AsWildcard();
		}

		[Fact]
		public async Task ChangeDescription_HasName_ShouldUsePluralVerb()
		{
			ChangeDescription[] changes = [ChangeDescriptionTests.Capture(fs => fs.File.WriteAllText("foo.txt", "")),];

			async Task Act()
			{
				await ThatAll(changes, c => c.HasName("bar.txt"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have name equal to \"bar.txt\" for all items,*").AsWildcard();
		}

		[Fact]
		public async Task ChangeDescription_HasNotifyFilters_ShouldUsePluralVerb()
		{
			ChangeDescription[] changes = [ChangeDescriptionTests.Capture(fs => fs.File.WriteAllText("foo.txt", "")),];

			async Task Act()
			{
				await ThatAll(changes, c => c.HasNotifyFilters(NotifyFilters.Security));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have notify filters Security for all items,*").AsWildcard();
		}

		[Fact]
		public async Task FileVersionInfo_HasInt32Property_ShouldUsePluralVerb()
		{
			IFileVersionInfo[] infos = [CreateFileVersionInfo(),];

			async Task Act()
			{
				await ThatAll(infos, v => v.DoesNotComplyWith(x => x.HasFileBuildPart(0)));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items do not have file build part 0 for all items,*").AsWildcard();
		}

		[Fact]
		public async Task FileVersionInfo_HasStringProperty_ShouldUsePluralVerb()
		{
			IFileVersionInfo[] infos = [CreateFileVersionInfo(),];

			async Task Act()
			{
				await ThatAll(infos, v => v.HasCompanyName("Contoso"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have company name equal to \"Contoso\" for all items,*").AsWildcard();
		}

#if NET8_0_OR_GREATER
		[Fact]
		public async Task FileSystem_HasDrive_ShouldUsePluralVerb()
		{
			MockFileSystem[] fileSystems = [new(o => o.SimulatingOperatingSystem(SimulationMode.Windows)),];

			async Task Act()
			{
				await ThatAll(fileSystems, fs => fs.HasDrive("Z:\\"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have drive \"Z:\\\\\" for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasAvailableFreeSpace_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasAvailableFreeSpace(99));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have available free space 99 for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasDriveFormat_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasDriveFormat("FAT32"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have drive format equal to \"FAT32\" for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasDriveType_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasDriveType(DriveType.Network));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have drive type Network for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasName_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasName("Z:\\"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have name equal to \"Z:\\\\\" for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasTotalFreeSpace_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasTotalFreeSpace(99));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have total free space 99 for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasTotalSize_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.DoesNotComplyWith(x => x.HasTotalSize(2048)));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items do not have total size 2048 for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_HasVolumeLabel_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.HasVolumeLabel("Abbey Road"));
			}

			await That(Act).Throws()
				.WithMessage("*whose Items have volume label equal to \"Abbey Road\" for all items,*").AsWildcard();
		}

		[Fact]
		public async Task DriveInfo_IsReady_ShouldUsePluralVerb()
		{
			IDriveInfo[] drives = [CreateDriveInfo(),];

			async Task Act()
			{
				await ThatAll(drives, d => d.IsNotReady());
			}

			await That(Act).Throws()
				.WithMessage("*whose Items are not ready for all items,*").AsWildcard();
		}

		private static IDriveInfo CreateDriveInfo()
		{
			MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
			fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));
			return fileSystem.DriveInfo.New("D:");
		}
#endif

		private static async Task ThatAll<T>(T[] items, Action<IThat<T>> expectations)
			{
			Container<T> container = new(items);
			await That(container).Whose(c => c.Items, i => i.All().ComplyWith(expectations));
		}

		private static IFileVersionInfo CreateFileVersionInfo()
		{
			MockFileSystem fileSystem = new();
			fileSystem.WithFileVersionInfo("*.dll", v => v.SetCompanyName("Acme"));
			// ReSharper disable once MethodHasAsyncOverload
			fileSystem.File.WriteAllText("Acme.dll", "");
			return fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");
		}
	}
}

file sealed class Container<T>(T[] items)
{
	public T[] Items { get; } = items;
}
