using System.IO.Abstractions;
using aweXpect.Core;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded property accesses against a single <see cref="IFileVersionInfo" /> instance
///     (identified by file name).
/// </summary>
public sealed class RecordedFileVersionInfoInstance
{
	private readonly string _bucketDescription;
	private readonly string _fileName;
	private readonly IThat<IFileSystemStatistics?> _subject;

	internal RecordedFileVersionInfoInstance(IThat<IFileSystemStatistics?> subject, string fileName)
	{
		_subject = subject;
		_fileName = fileName;
		_bucketDescription = $"FileVersionInfo[\"{fileName}\"]";
	}

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.Comments" />.</summary>
	public RecordedProperty Comments => Property(nameof(IFileVersionInfo.Comments));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.CompanyName" />.</summary>
	public RecordedProperty CompanyName => Property(nameof(IFileVersionInfo.CompanyName));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileBuildPart" />.</summary>
	public RecordedProperty FileBuildPart => Property(nameof(IFileVersionInfo.FileBuildPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileDescription" />.</summary>
	public RecordedProperty FileDescription => Property(nameof(IFileVersionInfo.FileDescription));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileMajorPart" />.</summary>
	public RecordedProperty FileMajorPart => Property(nameof(IFileVersionInfo.FileMajorPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileMinorPart" />.</summary>
	public RecordedProperty FileMinorPart => Property(nameof(IFileVersionInfo.FileMinorPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileName" />.</summary>
	public RecordedProperty FileName => Property(nameof(IFileVersionInfo.FileName));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FilePrivatePart" />.</summary>
	public RecordedProperty FilePrivatePart => Property(nameof(IFileVersionInfo.FilePrivatePart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.FileVersion" />.</summary>
	public RecordedProperty FileVersion => Property(nameof(IFileVersionInfo.FileVersion));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.InternalName" />.</summary>
	public RecordedProperty InternalName => Property(nameof(IFileVersionInfo.InternalName));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.IsDebug" />.</summary>
	public RecordedProperty IsDebug => Property(nameof(IFileVersionInfo.IsDebug));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.IsPatched" />.</summary>
	public RecordedProperty IsPatched => Property(nameof(IFileVersionInfo.IsPatched));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.IsPreRelease" />.</summary>
	public RecordedProperty IsPreRelease => Property(nameof(IFileVersionInfo.IsPreRelease));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.IsPrivateBuild" />.</summary>
	public RecordedProperty IsPrivateBuild => Property(nameof(IFileVersionInfo.IsPrivateBuild));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.IsSpecialBuild" />.</summary>
	public RecordedProperty IsSpecialBuild => Property(nameof(IFileVersionInfo.IsSpecialBuild));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.Language" />.</summary>
	public RecordedProperty Language => Property(nameof(IFileVersionInfo.Language));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.LegalCopyright" />.</summary>
	public RecordedProperty LegalCopyright => Property(nameof(IFileVersionInfo.LegalCopyright));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.LegalTrademarks" />.</summary>
	public RecordedProperty LegalTrademarks => Property(nameof(IFileVersionInfo.LegalTrademarks));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.OriginalFilename" />.</summary>
	public RecordedProperty OriginalFilename => Property(nameof(IFileVersionInfo.OriginalFilename));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.PrivateBuild" />.</summary>
	public RecordedProperty PrivateBuild => Property(nameof(IFileVersionInfo.PrivateBuild));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductBuildPart" />.</summary>
	public RecordedProperty ProductBuildPart => Property(nameof(IFileVersionInfo.ProductBuildPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductMajorPart" />.</summary>
	public RecordedProperty ProductMajorPart => Property(nameof(IFileVersionInfo.ProductMajorPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductMinorPart" />.</summary>
	public RecordedProperty ProductMinorPart => Property(nameof(IFileVersionInfo.ProductMinorPart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductName" />.</summary>
	public RecordedProperty ProductName => Property(nameof(IFileVersionInfo.ProductName));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductPrivatePart" />.</summary>
	public RecordedProperty ProductPrivatePart => Property(nameof(IFileVersionInfo.ProductPrivatePart));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.ProductVersion" />.</summary>
	public RecordedProperty ProductVersion => Property(nameof(IFileVersionInfo.ProductVersion));

	/// <summary>Recorded accesses to <see cref="IFileVersionInfo.SpecialBuild" />.</summary>
	public RecordedProperty SpecialBuild => Property(nameof(IFileVersionInfo.SpecialBuild));

	private RecordedProperty Property(string propertyName)
	{
		string fileName = _fileName;
		return new RecordedProperty(_subject, s => s.FileVersionInfo[fileName], _bucketDescription, propertyName);
	}
}
