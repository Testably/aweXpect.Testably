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
///     Assertions on recorded calls against <see cref="IFileInfoFactory" /> (factory-level)
///     and per-instance assertions accessed via the path indexer.
/// </summary>
public sealed class RecordedFileInfoBucket
{
	private const string BucketDescription = "FileInfo";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileInfoBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded calls against the <see cref="IFileInfo" /> instance for <paramref name="path" />.
	/// </summary>
	public RecordedFileInfoInstance this[string path]
	{
		get
		{
			ThrowHelper.ThrowIfNull(path, nameof(path));

			return new RecordedFileInfoInstance(_subject, path);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IFileInfoFactory.New(string)" />.
	/// </summary>
	public RecordedMethodCallResult New(
		Func<string, bool>? fileName = null,
		[CallerArgumentExpression(nameof(fileName))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFileInfoFactory.New),
			ParameterMatcher.From("fileName", fileName, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IFileInfoFactory.Wrap(System.IO.FileInfo?)" />.
	/// </summary>
	public RecordedMethodCallResult Wrap(
		Func<FileInfo?, bool>? fileInfo = null,
		[CallerArgumentExpression(nameof(fileInfo))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFileInfoFactory.Wrap),
			ParameterMatcher.From("fileInfo", fileInfo, doNotPopulateThisValue));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.FileInfo, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
