using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against <see cref="IFile" />.
/// </summary>
/// <remarks>
///     One mirror method per <see cref="IFile" /> method name. Parameters are <see cref="Func{T,TResult}" />? predicates;
///     pass <see langword="null" /> (or omit) to skip filtering on that parameter.
/// </remarks>
public sealed class RecordedFileMethods
{
	private const string BucketDescription = "File";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileMethods(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>Recorded calls to <c>IFile.AppendAllBytes(string, byte[])</c> and overloads.</summary>
	/// <remarks>
	///     The <c>ReadOnlySpan&lt;byte&gt;</c> overload records its bytes as a span; the <paramref name="bytes" />
	///     filter only matches the <c>byte[]</c> overload. Use no filter to count both.
	/// </remarks>
	public RecordedMethodCallResult AppendAllBytes(
		Func<string, bool>? path = null,
		Func<byte[], bool>? bytes = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(bytes))]
		string? bytesExpression = null)
		=> Build("AppendAllBytes",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("bytes", bytes, bytesExpression));

	/// <summary>Recorded calls to <c>IFile.AppendAllBytesAsync(string, byte[], CancellationToken)</c> and overloads.</summary>
	/// <remarks>
	///     The <c>ReadOnlyMemory&lt;byte&gt;</c> overload records its bytes as a memory value; the <paramref name="bytes" />
	///     filter only matches the <c>byte[]</c> overload. Use no filter to count both.
	/// </remarks>
	public RecordedMethodCallResult AppendAllBytesAsync(
		Func<string, bool>? path = null,
		Func<byte[], bool>? bytes = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(bytes))]
		string? bytesExpression = null)
		=> Build("AppendAllBytesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("bytes", bytes, bytesExpression));

	/// <summary>Recorded calls to <see cref="IFile.AppendAllLines(string, System.Collections.Generic.IEnumerable{string})" /> and overloads.</summary>
	public RecordedMethodCallResult AppendAllLines(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.AppendAllLines),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From<object>("contents", null, null),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.AppendAllLinesAsync(string, IEnumerable&lt;string&gt;, CancellationToken)</c> and overloads.</summary>
	/// <remarks>
	///     The contents parameter is omitted because the underlying overloads vary in generic shape; only
	///     <paramref name="path" />, <paramref name="encoding" />, and call count can be filtered.
	/// </remarks>
	public RecordedMethodCallResult AppendAllLinesAsync(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("AppendAllLinesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From<object>("contents", null, null),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.AppendAllText(string, string?)" /> and overloads.</summary>
	public RecordedMethodCallResult AppendAllText(
		Func<string, bool>? path = null,
		Func<string?, bool>? contents = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(contents))]
		string? contentsExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.AppendAllText),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("contents", contents, contentsExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.AppendAllTextAsync(string, string?, CancellationToken)</c> and overloads.</summary>
	public RecordedMethodCallResult AppendAllTextAsync(
		Func<string, bool>? path = null,
		Func<string?, bool>? contents = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(contents))]
		string? contentsExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("AppendAllTextAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("contents", contents, contentsExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.AppendText(string)" />.</summary>
	public RecordedMethodCallResult AppendText(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.AppendText),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.Copy(string, string)" /> and overloads.</summary>
	public RecordedMethodCallResult Copy(
		Func<string, bool>? sourceFileName = null,
		Func<string, bool>? destFileName = null,
		Func<bool, bool>? overwrite = null,
		[CallerArgumentExpression(nameof(sourceFileName))]
		string? sourceFileNameExpression = null,
		[CallerArgumentExpression(nameof(destFileName))]
		string? destFileNameExpression = null,
		[CallerArgumentExpression(nameof(overwrite))]
		string? overwriteExpression = null)
		=> Build(nameof(IFile.Copy),
			ParameterMatcher.From("sourceFileName", sourceFileName, sourceFileNameExpression),
			ParameterMatcher.From("destFileName", destFileName, destFileNameExpression),
			ParameterMatcher.From("overwrite", overwrite, overwriteExpression));

	/// <summary>Recorded calls to <see cref="IFile.Create(string)" /> and overloads.</summary>
	public RecordedMethodCallResult Create(
		Func<string, bool>? path = null,
		Func<int, bool>? bufferSize = null,
		Func<FileOptions, bool>? options = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(bufferSize))]
		string? bufferSizeExpression = null,
		[CallerArgumentExpression(nameof(options))]
		string? optionsExpression = null)
		=> Build(nameof(IFile.Create),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("bufferSize", bufferSize, bufferSizeExpression),
			ParameterMatcher.From("options", options, optionsExpression));

	/// <summary>Recorded calls to <c>IFile.CreateSymbolicLink(string, string)</c>.</summary>
	public RecordedMethodCallResult CreateSymbolicLink(
		Func<string, bool>? path = null,
		Func<string, bool>? pathToTarget = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(pathToTarget))]
		string? pathToTargetExpression = null)
		=> Build("CreateSymbolicLink",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("pathToTarget", pathToTarget, pathToTargetExpression));

	/// <summary>Recorded calls to <see cref="IFile.CreateText(string)" />.</summary>
	public RecordedMethodCallResult CreateText(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.CreateText),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.Decrypt(string)" />.</summary>
	public RecordedMethodCallResult Decrypt(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.Decrypt),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.Delete(string)" />.</summary>
	public RecordedMethodCallResult Delete(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.Delete),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.Encrypt(string)" />.</summary>
	public RecordedMethodCallResult Encrypt(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.Encrypt),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.Exists(string?)" />.</summary>
	public RecordedMethodCallResult Exists(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.Exists),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetAttributes(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetAttributes(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetAttributes),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetCreationTime(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetCreationTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetCreationTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetCreationTimeUtc(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetCreationTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetCreationTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetLastAccessTime(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetLastAccessTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetLastAccessTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetLastAccessTimeUtc(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetLastAccessTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetLastAccessTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetLastWriteTime(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetLastWriteTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetLastWriteTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.GetLastWriteTimeUtc(string)" /> and overloads.</summary>
	public RecordedMethodCallResult GetLastWriteTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.GetLastWriteTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

#if NET7_0_OR_GREATER
	/// <summary>Recorded calls to <c>IFile.GetUnixFileMode(string)</c>.</summary>
	public RecordedMethodCallResult GetUnixFileMode(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))] string? doNotPopulateThisValue = null)
		=> Build("GetUnixFileMode",
			ParameterMatcher.From("path", path, doNotPopulateThisValue));
#endif

	/// <summary>Recorded calls to <see cref="IFile.Move(string, string)" /> and overloads.</summary>
	public RecordedMethodCallResult Move(
		Func<string, bool>? sourceFileName = null,
		Func<string, bool>? destFileName = null,
		Func<bool, bool>? overwrite = null,
		[CallerArgumentExpression(nameof(sourceFileName))]
		string? sourceFileNameExpression = null,
		[CallerArgumentExpression(nameof(destFileName))]
		string? destFileNameExpression = null,
		[CallerArgumentExpression(nameof(overwrite))]
		string? overwriteExpression = null)
		=> Build(nameof(IFile.Move),
			ParameterMatcher.From("sourceFileName", sourceFileName, sourceFileNameExpression),
			ParameterMatcher.From("destFileName", destFileName, destFileNameExpression),
			ParameterMatcher.From("overwrite", overwrite, overwriteExpression));

	/// <summary>Recorded calls to <see cref="IFile.Open(string, FileMode)" /> and overloads.</summary>
	/// <remarks>
	///     The <c>Open(string, FileStreamOptions)</c> overload is matched (and counted) when no filter is supplied,
	///     but its <c>FileStreamOptions</c> value cannot be filtered through this mirror because it is recorded at
	///     parameter position 1 (the same slot used by <see cref="FileMode" /> on the other overloads).
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult Open(
		Func<string, bool>? path = null,
		Func<FileMode, bool>? mode = null,
		Func<FileAccess, bool>? access = null,
		Func<FileShare, bool>? share = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(mode))]
		string? modeExpression = null,
		[CallerArgumentExpression(nameof(access))]
		string? accessExpression = null,
		[CallerArgumentExpression(nameof(share))]
		string? shareExpression = null)
		=> Build(nameof(IFile.Open),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("mode", mode, modeExpression),
			ParameterMatcher.From("access", access, accessExpression),
			ParameterMatcher.From("share", share, shareExpression));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IFile.OpenRead(string)" />.</summary>
	public RecordedMethodCallResult OpenRead(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.OpenRead),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.OpenText(string)" />.</summary>
	public RecordedMethodCallResult OpenText(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.OpenText),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.OpenWrite(string)" />.</summary>
	public RecordedMethodCallResult OpenWrite(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.OpenWrite),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.ReadAllBytes(string)" />.</summary>
	public RecordedMethodCallResult ReadAllBytes(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFile.ReadAllBytes),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <c>IFile.ReadAllBytesAsync(string, CancellationToken)</c>.</summary>
	public RecordedMethodCallResult ReadAllBytesAsync(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build("ReadAllBytesAsync",
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IFile.ReadAllLines(string)" /> and overloads.</summary>
	public RecordedMethodCallResult ReadAllLines(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.ReadAllLines),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.ReadAllLinesAsync(string, CancellationToken)</c> and overloads.</summary>
	public RecordedMethodCallResult ReadAllLinesAsync(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("ReadAllLinesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.ReadAllText(string)" /> and overloads.</summary>
	public RecordedMethodCallResult ReadAllText(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.ReadAllText),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.ReadAllTextAsync(string, CancellationToken)</c> and overloads.</summary>
	public RecordedMethodCallResult ReadAllTextAsync(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("ReadAllTextAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.ReadLines(string)" /> and overloads.</summary>
	public RecordedMethodCallResult ReadLines(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.ReadLines),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.ReadLinesAsync(string, CancellationToken)</c> and overloads.</summary>
	public RecordedMethodCallResult ReadLinesAsync(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("ReadLinesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.Replace(string, string, string?)" /> and overloads.</summary>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult Replace(
		Func<string, bool>? sourceFileName = null,
		Func<string, bool>? destinationFileName = null,
		Func<string?, bool>? destinationBackupFileName = null,
		Func<bool, bool>? ignoreMetadataErrors = null,
		[CallerArgumentExpression(nameof(sourceFileName))]
		string? sourceFileNameExpression = null,
		[CallerArgumentExpression(nameof(destinationFileName))]
		string? destinationFileNameExpression = null,
		[CallerArgumentExpression(nameof(destinationBackupFileName))]
		string? destinationBackupFileNameExpression = null,
		[CallerArgumentExpression(nameof(ignoreMetadataErrors))]
		string? ignoreMetadataErrorsExpression = null)
		=> Build(nameof(IFile.Replace),
			ParameterMatcher.From("sourceFileName", sourceFileName, sourceFileNameExpression),
			ParameterMatcher.From("destinationFileName", destinationFileName, destinationFileNameExpression),
			ParameterMatcher.From("destinationBackupFileName", destinationBackupFileName, destinationBackupFileNameExpression),
			ParameterMatcher.From("ignoreMetadataErrors", ignoreMetadataErrors, ignoreMetadataErrorsExpression));
#pragma warning restore S107

	/// <summary>Recorded calls to <c>IFile.ResolveLinkTarget(string, bool)</c>.</summary>
	public RecordedMethodCallResult ResolveLinkTarget(
		Func<string, bool>? linkPath = null,
		Func<bool, bool>? returnFinalTarget = null,
		[CallerArgumentExpression(nameof(linkPath))]
		string? linkPathExpression = null,
		[CallerArgumentExpression(nameof(returnFinalTarget))]
		string? returnFinalTargetExpression = null)
		=> Build("ResolveLinkTarget",
			ParameterMatcher.From("linkPath", linkPath, linkPathExpression),
			ParameterMatcher.From("returnFinalTarget", returnFinalTarget, returnFinalTargetExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetAttributes(string, FileAttributes)" /> and overloads.</summary>
	public RecordedMethodCallResult SetAttributes(
		Func<string, bool>? path = null,
		Func<FileAttributes, bool>? fileAttributes = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(fileAttributes))]
		string? fileAttributesExpression = null)
		=> Build(nameof(IFile.SetAttributes),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("fileAttributes", fileAttributes, fileAttributesExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetCreationTime(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetCreationTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? creationTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(creationTime))]
		string? creationTimeExpression = null)
		=> Build(nameof(IFile.SetCreationTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("creationTime", creationTime, creationTimeExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetCreationTimeUtc(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetCreationTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? creationTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(creationTimeUtc))]
		string? creationTimeUtcExpression = null)
		=> Build(nameof(IFile.SetCreationTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("creationTimeUtc", creationTimeUtc, creationTimeUtcExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetLastAccessTime(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetLastAccessTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastAccessTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastAccessTime))]
		string? lastAccessTimeExpression = null)
		=> Build(nameof(IFile.SetLastAccessTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastAccessTime", lastAccessTime, lastAccessTimeExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetLastAccessTimeUtc(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetLastAccessTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastAccessTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastAccessTimeUtc))]
		string? lastAccessTimeUtcExpression = null)
		=> Build(nameof(IFile.SetLastAccessTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastAccessTimeUtc", lastAccessTimeUtc, lastAccessTimeUtcExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetLastWriteTime(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetLastWriteTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastWriteTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastWriteTime))]
		string? lastWriteTimeExpression = null)
		=> Build(nameof(IFile.SetLastWriteTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastWriteTime", lastWriteTime, lastWriteTimeExpression));

	/// <summary>Recorded calls to <see cref="IFile.SetLastWriteTimeUtc(string, DateTime)" /> and overloads.</summary>
	public RecordedMethodCallResult SetLastWriteTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastWriteTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastWriteTimeUtc))]
		string? lastWriteTimeUtcExpression = null)
		=> Build(nameof(IFile.SetLastWriteTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastWriteTimeUtc", lastWriteTimeUtc, lastWriteTimeUtcExpression));

#if NET7_0_OR_GREATER
	/// <summary>Recorded calls to <c>IFile.SetUnixFileMode(string, UnixFileMode)</c>.</summary>
	public RecordedMethodCallResult SetUnixFileMode(
		Func<string, bool>? path = null,
		Func<UnixFileMode, bool>? mode = null,
		[CallerArgumentExpression(nameof(path))] string? pathExpression = null,
		[CallerArgumentExpression(nameof(mode))] string? modeExpression = null)
		=> Build("SetUnixFileMode",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("mode", mode, modeExpression));
#endif

	/// <summary>Recorded calls to <see cref="IFile.WriteAllBytes(string, byte[])" /> and overloads.</summary>
	public RecordedMethodCallResult WriteAllBytes(
		Func<string, bool>? path = null,
		Func<byte[], bool>? bytes = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(bytes))]
		string? bytesExpression = null)
		=> Build(nameof(IFile.WriteAllBytes),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("bytes", bytes, bytesExpression));

	/// <summary>Recorded calls to <c>IFile.WriteAllBytesAsync(string, byte[], CancellationToken)</c> and overloads.</summary>
	/// <remarks>
	///     The <c>ReadOnlyMemory&lt;byte&gt;</c> overload records its bytes as a memory value; the <paramref name="bytes" />
	///     filter only matches the <c>byte[]</c> overload. Use no filter to count both.
	/// </remarks>
	public RecordedMethodCallResult WriteAllBytesAsync(
		Func<string, bool>? path = null,
		Func<byte[], bool>? bytes = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(bytes))]
		string? bytesExpression = null)
		=> Build("WriteAllBytesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("bytes", bytes, bytesExpression));

	/// <summary>Recorded calls to <see cref="IFile.WriteAllLines(string, System.Collections.Generic.IEnumerable{string})" /> and overloads.</summary>
	public RecordedMethodCallResult WriteAllLines(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.WriteAllLines),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From<object>("contents", null, null),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.WriteAllLinesAsync(string, IEnumerable&lt;string&gt;, CancellationToken)</c> and overloads.</summary>
	/// <remarks>
	///     The contents parameter is omitted because the underlying overloads vary in generic shape; only
	///     <paramref name="path" />, <paramref name="encoding" />, and call count can be filtered.
	/// </remarks>
	public RecordedMethodCallResult WriteAllLinesAsync(
		Func<string, bool>? path = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("WriteAllLinesAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From<object>("contents", null, null),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <see cref="IFile.WriteAllText(string, string?)" /> and overloads.</summary>
	public RecordedMethodCallResult WriteAllText(
		Func<string, bool>? path = null,
		Func<string?, bool>? contents = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(contents))]
		string? contentsExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build(nameof(IFile.WriteAllText),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("contents", contents, contentsExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	/// <summary>Recorded calls to <c>IFile.WriteAllTextAsync(string, string?, CancellationToken)</c> and overloads.</summary>
	public RecordedMethodCallResult WriteAllTextAsync(
		Func<string, bool>? path = null,
		Func<string?, bool>? contents = null,
		Func<Encoding, bool>? encoding = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(contents))]
		string? contentsExpression = null,
		[CallerArgumentExpression(nameof(encoding))]
		string? encodingExpression = null)
		=> Build("WriteAllTextAsync",
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("contents", contents, contentsExpression),
			ParameterMatcher.From("encoding", encoding, encodingExpression));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.File, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
