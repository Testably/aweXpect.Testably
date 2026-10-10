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
///     Assertions on recorded calls against <see cref="IDirectoryInfoFactory" /> (factory-level)
///     and per-instance assertions accessed via the path indexer.
/// </summary>
public sealed class RecordedDirectoryInfoBucket
{
	private const string BucketDescription = "DirectoryInfo";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedDirectoryInfoBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded calls against the <see cref="IDirectoryInfo" /> instance for <paramref name="path" />.
	/// </summary>
	public RecordedDirectoryInfoInstance this[string path]
	{
		get
		{
			ThrowHelper.ThrowIfNull(path, nameof(path));

			return new RecordedDirectoryInfoInstance(_subject, path);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfoFactory.New(string)" />.
	/// </summary>
	public RecordedMethodCallResult New(
		Func<string, bool>? path = null,
		[CallerArgumentExpression(nameof(path))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectoryInfoFactory.New),
			ParameterMatcher.From("path", path, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IDirectoryInfoFactory.Wrap(System.IO.DirectoryInfo?)" />.
	/// </summary>
	public RecordedMethodCallResult Wrap(
		Func<DirectoryInfo?, bool>? directoryInfo = null,
		[CallerArgumentExpression(nameof(directoryInfo))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDirectoryInfoFactory.Wrap),
			ParameterMatcher.From("directoryInfo", directoryInfo, doNotPopulateThisValue));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.DirectoryInfo, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
