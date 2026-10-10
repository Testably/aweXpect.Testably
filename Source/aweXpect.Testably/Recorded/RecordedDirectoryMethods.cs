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
///     Assertions on recorded calls against <see cref="IDirectory" />.
/// </summary>
public sealed class RecordedDirectoryMethods
{
	private const string BucketDescription = "Directory";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedDirectoryMethods(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>Recorded calls to <see cref="IDirectory.CreateDirectory(string)" /> and overloads.</summary>
	public RecordedMethodCallResult CreateDirectory(
		Func<string, bool>? path = null,
#if NET7_0_OR_GREATER
		Func<UnixFileMode, bool>? unixCreateMode = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null
#if NET7_0_OR_GREATER
		, [CallerArgumentExpression(nameof(unixCreateMode))] string? unixCreateModeExpression = null
#endif
	)
		=> Build(nameof(IDirectory.CreateDirectory),
			ParameterMatcher.From("path", path, pathExpression)
#if NET7_0_OR_GREATER
			, ParameterMatcher.From("unixCreateMode", unixCreateMode, unixCreateModeExpression)
#endif
		);

	/// <summary>Recorded calls to <c>IDirectory.CreateSymbolicLink(string, string)</c>.</summary>
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

	/// <summary>Recorded calls to <c>IDirectory.CreateTempSubdirectory(string?)</c>.</summary>
	public RecordedMethodCallResult CreateTempSubdirectory(
		Func<string?, bool>? prefix = null,
		[CallerArgumentExpression(nameof(prefix))]
		string? doNotPopulateThisValue = null)
		=> Build("CreateTempSubdirectory",
			ParameterMatcher.From("prefix", prefix, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.Delete(string)" /> and overloads.</summary>
	public RecordedMethodCallResult Delete(
		Func<string, bool>? path = null,
		Func<bool, bool>? recursive = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(recursive))]
		string? recursiveExpression = null)
		=> Build(nameof(IDirectory.Delete),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("recursive", recursive, recursiveExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.EnumerateDirectories(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult EnumerateDirectories(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.EnumerateDirectories),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.EnumerateFiles(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult EnumerateFiles(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.EnumerateFiles),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.EnumerateFileSystemEntries(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult EnumerateFileSystemEntries(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.EnumerateFileSystemEntries),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.Exists(string?)" />.</summary>
	public RecordedMethodCallResult Exists(
		Func<string?, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.Exists),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetCreationTime(string)" />.</summary>
	public RecordedMethodCallResult GetCreationTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetCreationTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetCreationTimeUtc(string)" />.</summary>
	public RecordedMethodCallResult GetCreationTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetCreationTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetCurrentDirectory" />.</summary>
	public RecordedMethodCallResult GetCurrentDirectory()
		=> Build(nameof(IDirectory.GetCurrentDirectory));

	/// <summary>Recorded calls to <see cref="IDirectory.GetDirectories(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult GetDirectories(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.GetDirectories),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.GetDirectoryRoot(string)" />.</summary>
	public RecordedMethodCallResult GetDirectoryRoot(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetDirectoryRoot),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetFiles(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult GetFiles(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.GetFiles),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.GetFileSystemEntries(string)" /> and overloads.</summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 2,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult GetFileSystemEntries(
		Func<string, bool>? path = null,
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectory.GetFileSystemEntries),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));
#pragma warning restore S107

	/// <summary>Recorded calls to <see cref="IDirectory.GetLastAccessTime(string)" />.</summary>
	public RecordedMethodCallResult GetLastAccessTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetLastAccessTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetLastAccessTimeUtc(string)" />.</summary>
	public RecordedMethodCallResult GetLastAccessTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetLastAccessTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetLastWriteTime(string)" />.</summary>
	public RecordedMethodCallResult GetLastWriteTime(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetLastWriteTime),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetLastWriteTimeUtc(string)" />.</summary>
	public RecordedMethodCallResult GetLastWriteTimeUtc(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetLastWriteTimeUtc),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.GetLogicalDrives" />.</summary>
	public RecordedMethodCallResult GetLogicalDrives()
		=> Build(nameof(IDirectory.GetLogicalDrives));

	/// <summary>Recorded calls to <see cref="IDirectory.GetParent(string)" />.</summary>
	public RecordedMethodCallResult GetParent(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.GetParent),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.Move(string, string)" />.</summary>
	public RecordedMethodCallResult Move(
		Func<string, bool>? sourceDirName = null,
		Func<string, bool>? destDirName = null,
		[CallerArgumentExpression(nameof(sourceDirName))]
		string? sourceDirNameExpression = null,
		[CallerArgumentExpression(nameof(destDirName))]
		string? destDirNameExpression = null)
		=> Build(nameof(IDirectory.Move),
			ParameterMatcher.From("sourceDirName", sourceDirName, sourceDirNameExpression),
			ParameterMatcher.From("destDirName", destDirName, destDirNameExpression));

	/// <summary>Recorded calls to <c>IDirectory.ResolveLinkTarget(string, bool)</c>.</summary>
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

	/// <summary>Recorded calls to <see cref="IDirectory.SetCreationTime(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetCreationTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? creationTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(creationTime))]
		string? creationTimeExpression = null)
		=> Build(nameof(IDirectory.SetCreationTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("creationTime", creationTime, creationTimeExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.SetCreationTimeUtc(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetCreationTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? creationTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(creationTimeUtc))]
		string? creationTimeUtcExpression = null)
		=> Build(nameof(IDirectory.SetCreationTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("creationTimeUtc", creationTimeUtc, creationTimeUtcExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.SetCurrentDirectory(string)" />.</summary>
	public RecordedMethodCallResult SetCurrentDirectory(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectory.SetCurrentDirectory),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="IDirectory.SetLastAccessTime(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetLastAccessTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastAccessTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastAccessTime))]
		string? lastAccessTimeExpression = null)
		=> Build(nameof(IDirectory.SetLastAccessTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastAccessTime", lastAccessTime, lastAccessTimeExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.SetLastAccessTimeUtc(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetLastAccessTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastAccessTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastAccessTimeUtc))]
		string? lastAccessTimeUtcExpression = null)
		=> Build(nameof(IDirectory.SetLastAccessTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastAccessTimeUtc", lastAccessTimeUtc, lastAccessTimeUtcExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.SetLastWriteTime(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetLastWriteTime(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastWriteTime = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastWriteTime))]
		string? lastWriteTimeExpression = null)
		=> Build(nameof(IDirectory.SetLastWriteTime),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastWriteTime", lastWriteTime, lastWriteTimeExpression));

	/// <summary>Recorded calls to <see cref="IDirectory.SetLastWriteTimeUtc(string, DateTime)" />.</summary>
	public RecordedMethodCallResult SetLastWriteTimeUtc(
		Func<string, bool>? path = null,
		Func<DateTime, bool>? lastWriteTimeUtc = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(lastWriteTimeUtc))]
		string? lastWriteTimeUtcExpression = null)
		=> Build(nameof(IDirectory.SetLastWriteTimeUtc),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("lastWriteTimeUtc", lastWriteTimeUtc, lastWriteTimeUtcExpression));

	private static ParameterMatcher SearchOrEnumerationMatcher(
		Func<SearchOption, bool>? searchOption,
		string? searchOptionExpression
#if NET6_0_OR_GREATER
		, Func<EnumerationOptions, bool>? enumerationOptions,
		string? enumerationOptionsExpression
#endif
	)
	{
#if NET6_0_OR_GREATER
		if (enumerationOptions is not null)
		{
			return ParameterMatcher.From("enumerationOptions", enumerationOptions, enumerationOptionsExpression);
		}
#endif
		return ParameterMatcher.From("searchOption", searchOption, searchOptionExpression);
	}

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.Directory, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
