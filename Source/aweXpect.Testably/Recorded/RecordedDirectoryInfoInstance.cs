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
///     Assertions on recorded calls against a single <see cref="IDirectoryInfo" /> instance (identified by path).
/// </summary>
public sealed class RecordedDirectoryInfoInstance
{
	private readonly string _bucketDescription;
	private readonly string _path;
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedDirectoryInfoInstance(IThat<IFileSystemStatistics> subject, string path)
	{
		_subject = subject;
		_path = path;
		_bucketDescription = $"DirectoryInfo[\"{path}\"]";
	}

	/// <summary>
	///     Recorded accesses to <see cref="IFileSystemInfo.Attributes" />.
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
	///     Recorded accesses to <see cref="IDirectoryInfo.Parent" />.
	/// </summary>
	public RecordedProperty Parent
		=> Property(nameof(IDirectoryInfo.Parent));

	/// <summary>
	///     Recorded accesses to <see cref="IDirectoryInfo.Root" />.
	/// </summary>
	public RecordedProperty Root
		=> Property(nameof(IDirectoryInfo.Root));

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
	///     Recorded calls to <see cref="IDirectoryInfo.Create()" />.
	/// </summary>
	public RecordedMethodCallResult Create()
		=> Build(nameof(IDirectoryInfo.Create));

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
	///     Recorded calls to <see cref="IDirectoryInfo.CreateSubdirectory(string)" />.
	/// </summary>
	public RecordedMethodCallResult CreateSubdirectory(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectoryInfo.CreateSubdirectory),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemInfo.Delete" /> and <c>IDirectoryInfo.Delete(bool)</c>.
	/// </summary>
	public RecordedMethodCallResult Delete(
		Func<bool, bool>? recursive = null,
		[CallerArgumentExpression(nameof(recursive))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFileSystemInfo.Delete),
			ParameterMatcher.From("recursive", recursive, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.EnumerateDirectories()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult EnumerateDirectories(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.EnumerateDirectories),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.EnumerateFiles()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult EnumerateFiles(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.EnumerateFiles),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.EnumerateFileSystemInfos()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult EnumerateFileSystemInfos(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.EnumerateFileSystemInfos),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.GetDirectories()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult GetDirectories(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.GetDirectories),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.GetFiles()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult GetFiles(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.GetFiles),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.GetFileSystemInfos()" /> and overloads.
	/// </summary>
	/// <remarks>
	///     <paramref name="searchOption" /> and <c>enumerationOptions</c> share recording position 1,
	///     so only one should be supplied per call. If both are supplied, <c>enumerationOptions</c> wins.
	/// </remarks>
	public RecordedMethodCallResult GetFileSystemInfos(
		Func<string, bool>? searchPattern = null,
		Func<SearchOption, bool>? searchOption = null,
#if NET6_0_OR_GREATER
		Func<EnumerationOptions, bool>? enumerationOptions = null,
#endif
		[CallerArgumentExpression(nameof(searchPattern))]
		string? searchPatternExpression = null,
		[CallerArgumentExpression(nameof(searchOption))]
		string? searchOptionExpression = null
#if NET6_0_OR_GREATER
		, [CallerArgumentExpression(nameof(enumerationOptions))] string? enumerationOptionsExpression = null
#endif
	)
		=> Build(nameof(IDirectoryInfo.GetFileSystemInfos),
			ParameterMatcher.From("searchPattern", searchPattern, searchPatternExpression),
			SearchOrEnumerationMatcher(searchOption, searchOptionExpression
#if NET6_0_OR_GREATER
				, enumerationOptions, enumerationOptionsExpression
#endif
			));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfo.MoveTo(string)" />.
	/// </summary>
	public RecordedMethodCallResult MoveTo(
		Func<string, bool>? destDirName = null,
		[CallerArgumentExpression(nameof(destDirName))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectoryInfo.MoveTo),
			ParameterMatcher.From("destDirName", destDirName, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemInfo.Refresh" />.
	/// </summary>
	public RecordedMethodCallResult Refresh()
		=> Build(nameof(IFileSystemInfo.Refresh));

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
		return new RecordedProperty(_subject, s => s.DirectoryInfo[path], _bucketDescription, propertyName);
	}

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
		string path = _path;
		string bucketDescription = _bucketDescription;
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, path, bucketDescription, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.DirectoryInfo[state.path], state.bucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
