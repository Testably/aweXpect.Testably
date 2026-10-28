using System.IO.Abstractions;
using aweXpect.Core;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded property accesses against a single <see cref="IDriveInfo" /> instance
///     (identified by drive name).
/// </summary>
public sealed class RecordedDriveInfoInstance
{
	private readonly string _bucketDescription;
	private readonly string _driveName;
	private readonly IThat<IFileSystemStatistics?> _subject;

	internal RecordedDriveInfoInstance(IThat<IFileSystemStatistics?> subject, string driveName)
	{
		_subject = subject;
		_driveName = driveName;
		_bucketDescription = $"DriveInfo[\"{driveName}\"]";
	}

	/// <summary>Recorded accesses to <see cref="IDriveInfo.AvailableFreeSpace" />.</summary>
	public RecordedProperty AvailableFreeSpace => Property(nameof(IDriveInfo.AvailableFreeSpace));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.DriveFormat" />.</summary>
	public RecordedProperty DriveFormat => Property(nameof(IDriveInfo.DriveFormat));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.DriveType" />.</summary>
	public RecordedProperty DriveType => Property(nameof(IDriveInfo.DriveType));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.IsReady" />.</summary>
	public RecordedProperty IsReady => Property(nameof(IDriveInfo.IsReady));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.Name" />.</summary>
	public RecordedProperty Name => Property(nameof(IDriveInfo.Name));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.RootDirectory" />.</summary>
	public RecordedProperty RootDirectory => Property(nameof(IDriveInfo.RootDirectory));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.TotalFreeSpace" />.</summary>
	public RecordedProperty TotalFreeSpace => Property(nameof(IDriveInfo.TotalFreeSpace));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.TotalSize" />.</summary>
	public RecordedProperty TotalSize => Property(nameof(IDriveInfo.TotalSize));

	/// <summary>Recorded accesses to <see cref="IDriveInfo.VolumeLabel" />.</summary>
	public RecordedProperty VolumeLabel => Property(nameof(IDriveInfo.VolumeLabel));

	private RecordedProperty Property(string propertyName)
	{
		string driveName = _driveName;
		return new RecordedProperty(_subject, s => s.DriveInfo[driveName], _bucketDescription, propertyName);
	}
}
