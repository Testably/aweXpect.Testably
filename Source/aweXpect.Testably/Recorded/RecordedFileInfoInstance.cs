using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against a single <see cref="IFileInfo" /> instance (identified by path).
/// </summary>
public sealed class RecordedFileInfoInstance
{
	private readonly string _bucketDescription;
	private readonly string _path;
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileInfoInstance(IThat<IFileSystemStatistics> subject, string path)
	{
		_subject = subject;
		_path = path;
		_bucketDescription = $"FileInfo[\"{path}\"]";
	}

	/// <summary>
	///     Recorded accesses to <see cref="System.IO.FileSystemInfo.Attributes" /> via <see cref="IFileInfo" />.
	/// </summary>
	public RecordedProperty Attributes
		=> Property(nameof(IFileSystemInfo.Attributes));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.CreationTime" />.
	/// </summary>
	public RecordedProperty CreationTime
		=> Property(nameof(IFileSystemInfo.CreationTime));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.CreationTimeUtc" />.
	/// </summary>
	public RecordedProperty CreationTimeUtc
		=> Property(nameof(IFileSystemInfo.CreationTimeUtc));

	/// <summary>
	///     Recorded accesses to <see cref="IFileInfo.Directory" />.
	/// </summary>
	public RecordedProperty Directory
		=> Property(nameof(IFileInfo.Directory));

	/// <summary>
	///     Recorded accesses to <see cref="IFileInfo.DirectoryName" />.
	/// </summary>
	public RecordedProperty DirectoryName
		=> Property(nameof(IFileInfo.DirectoryName));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.Exists" />.
	/// </summary>
	public RecordedProperty Exists
		=> Property(nameof(IFileSystemInfo.Exists));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.Extension" />.
	/// </summary>
	public RecordedProperty Extension
		=> Property(nameof(IFileSystemInfo.Extension));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.FullName" />.
	/// </summary>
	public RecordedProperty FullName
		=> Property(nameof(IFileSystemInfo.FullName));

	/// <summary>
	///     Recorded accesses to <see cref="IFileInfo.IsReadOnly" />.
	/// </summary>
	public RecordedProperty IsReadOnly
		=> Property(nameof(IFileInfo.IsReadOnly));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.LastAccessTime" />.
	/// </summary>
	public RecordedProperty LastAccessTime
		=> Property(nameof(IFileSystemInfo.LastAccessTime));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.LastAccessTimeUtc" />.
	/// </summary>
	public RecordedProperty LastAccessTimeUtc
		=> Property(nameof(IFileSystemInfo.LastAccessTimeUtc));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.LastWriteTime" />.
	/// </summary>
	public RecordedProperty LastWriteTime
		=> Property(nameof(IFileSystemInfo.LastWriteTime));

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.LastWriteTimeUtc" />.
	/// </summary>
	public RecordedProperty LastWriteTimeUtc
		=> Property(nameof(IFileSystemInfo.LastWriteTimeUtc));

	/// <summary>
	///     Recorded accesses to <see cref="IFileInfo.Length" />.
	/// </summary>
	public RecordedProperty Length
		=> Property(nameof(IFileInfo.Length));

	/// <summary>
	///     Recorded accesses to <c>IFileSystemInfo.LinkTarget</c>.
	/// </summary>
	public RecordedProperty LinkTarget
#if NET6_0_OR_GREATER
		=> Property(nameof(IFileSystemInfo.LinkTarget));
#else
		=> Property("LinkTarget");
#endif

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.Name" />.
	/// </summary>
	public RecordedProperty Name
		=> Property(nameof(IFileSystemInfo.Name));

	/// <summary>
	///     Recorded accesses to <c>IFileSystemInfo.UnixFileMode</c>.
	/// </summary>
	public RecordedProperty UnixFileMode
#if NET7_0_OR_GREATER
		=> Property(nameof(IFileSystemInfo.UnixFileMode));
#else
		=> Property("UnixFileMode");
#endif

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.AppendText()" />.
	/// </summary>
	public RecordedMethodCallResult AppendText()
		=> Build(nameof(IFileInfo.AppendText));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.CopyTo(string)" /> and overloads.
	/// </summary>
	public RecordedMethodCallResult CopyTo(
		Func<string, bool>? destFileName = null,
		Func<bool, bool>? overwrite = null,
		[CallerArgumentExpression(nameof(destFileName))]
		string? destFileNameExpression = null,
		[CallerArgumentExpression(nameof(overwrite))]
		string? overwriteExpression = null)
		=> Build(nameof(IFileInfo.CopyTo),
			ParameterMatcher.From("destFileName", destFileName, destFileNameExpression),
			ParameterMatcher.From("overwrite", overwrite, overwriteExpression));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Create()" />.
	/// </summary>
	public RecordedMethodCallResult Create()
		=> Build(nameof(IFileInfo.Create));

	/// <summary>
	///     Recorded calls to <c>IFileSystemInfo.CreateAsSymbolicLink(string)</c>.
	/// </summary>
	public RecordedMethodCallResult CreateAsSymbolicLink(
		Func<string, bool>? pathToTarget = null,
		[CallerArgumentExpression(nameof(pathToTarget))]
		string? doNotPopulateThisValue = null)
		=> Build("CreateAsSymbolicLink",
			ParameterMatcher.From("pathToTarget", pathToTarget, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.CreateText()" />.
	/// </summary>
	public RecordedMethodCallResult CreateText()
		=> Build(nameof(IFileInfo.CreateText));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Decrypt()" />.
	/// </summary>
	public RecordedMethodCallResult Decrypt()
		=> Build(nameof(IFileInfo.Decrypt));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemInfo.Delete" />.
	/// </summary>
	public RecordedMethodCallResult Delete()
		=> Build(nameof(IFileSystemInfo.Delete));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Encrypt()" />.
	/// </summary>
	public RecordedMethodCallResult Encrypt()
		=> Build(nameof(IFileInfo.Encrypt));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.MoveTo(string)" /> and overloads.
	/// </summary>
	public RecordedMethodCallResult MoveTo(
		Func<string, bool>? destFileName = null,
		Func<bool, bool>? overwrite = null,
		[CallerArgumentExpression(nameof(destFileName))]
		string? destFileNameExpression = null,
		[CallerArgumentExpression(nameof(overwrite))]
		string? overwriteExpression = null)
		=> Build(nameof(IFileInfo.MoveTo),
			ParameterMatcher.From("destFileName", destFileName, destFileNameExpression),
			ParameterMatcher.From("overwrite", overwrite, overwriteExpression));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Open(FileMode)" /> and overloads.
	/// </summary>
	/// <remarks>
	///     The <c>Open(FileStreamOptions)</c> overload is counted in unfiltered calls, but its
	///     <c>FileStreamOptions</c> value cannot be filtered through this mirror because it is
	///     recorded at parameter position 0 (the same slot used by <see cref="FileMode" /> on the
	///     other overloads).
	/// </remarks>
	public RecordedMethodCallResult Open(
		Func<FileMode, bool>? mode = null,
		Func<FileAccess, bool>? access = null,
		Func<FileShare, bool>? share = null,
		[CallerArgumentExpression(nameof(mode))]
		string? modeExpression = null,
		[CallerArgumentExpression(nameof(access))]
		string? accessExpression = null,
		[CallerArgumentExpression(nameof(share))]
		string? shareExpression = null)
		=> Build(nameof(IFileInfo.Open),
			ParameterMatcher.From("mode", mode, modeExpression),
			ParameterMatcher.From("access", access, accessExpression),
			ParameterMatcher.From("share", share, shareExpression));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.OpenRead()" />.
	/// </summary>
	public RecordedMethodCallResult OpenRead()
		=> Build(nameof(IFileInfo.OpenRead));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.OpenText()" />.
	/// </summary>
	public RecordedMethodCallResult OpenText()
		=> Build(nameof(IFileInfo.OpenText));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.OpenWrite()" />.
	/// </summary>
	public RecordedMethodCallResult OpenWrite()
		=> Build(nameof(IFileInfo.OpenWrite));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemInfo.Refresh" />.
	/// </summary>
	public RecordedMethodCallResult Refresh()
		=> Build(nameof(IFileSystemInfo.Refresh));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfo.Replace(string, string?)" /> and overloads.
	/// </summary>
	public RecordedMethodCallResult Replace(
		Func<string, bool>? destinationFileName = null,
		Func<string?, bool>? destinationBackupFileName = null,
		Func<bool, bool>? ignoreMetadataErrors = null,
		[CallerArgumentExpression(nameof(destinationFileName))]
		string? destinationFileNameExpression = null,
		[CallerArgumentExpression(nameof(destinationBackupFileName))]
		string? destinationBackupFileNameExpression = null,
		[CallerArgumentExpression(nameof(ignoreMetadataErrors))]
		string? ignoreMetadataErrorsExpression = null)
		=> Build(nameof(IFileInfo.Replace),
			ParameterMatcher.From("destinationFileName", destinationFileName, destinationFileNameExpression),
			ParameterMatcher.From("destinationBackupFileName", destinationBackupFileName, destinationBackupFileNameExpression),
			ParameterMatcher.From("ignoreMetadataErrors", ignoreMetadataErrors, ignoreMetadataErrorsExpression));

	/// <summary>
	///     Recorded calls to <c>IFileSystemInfo.ResolveLinkTarget(bool)</c>.
	/// </summary>
	public RecordedMethodCallResult ResolveLinkTarget(
		Func<bool, bool>? returnFinalTarget = null,
		[CallerArgumentExpression(nameof(returnFinalTarget))]
		string? doNotPopulateThisValue = null)
		=> Build("ResolveLinkTarget",
			ParameterMatcher.From("returnFinalTarget", returnFinalTarget, doNotPopulateThisValue));

	private RecordedProperty Property(string propertyName)
	{
		string path = _path;
		return new RecordedProperty(_subject, s => s.FileInfo[path], _bucketDescription, propertyName);
	}

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		string path = _path;
		string bucketDescription = _bucketDescription;
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, path, bucketDescription, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.FileInfo[state.path], state.bucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
