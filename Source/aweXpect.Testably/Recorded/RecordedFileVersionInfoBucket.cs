using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against <see cref="IFileVersionInfoFactory" /> (factory-level)
///     and per-instance assertions accessed via the file-name indexer.
/// </summary>
public sealed class RecordedFileVersionInfoBucket
{
	private const string BucketDescription = "FileVersionInfo";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileVersionInfoBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded property accesses against the <see cref="IFileVersionInfo" /> instance for
	///     <paramref name="fileName" />.
	/// </summary>
	public RecordedFileVersionInfoInstance this[string fileName]
	{
		get
		{
			ThrowHelper.ThrowIfNull(fileName, nameof(fileName));

			return new RecordedFileVersionInfoInstance(_subject, fileName);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IFileVersionInfoFactory.GetVersionInfo(string)" />.
	/// </summary>
	public RecordedMethodCallResult GetVersionInfo(
		Func<string, bool>? fileName = null,
		[CallerArgumentExpression(nameof(fileName))]
		string? fileNameExpression = null)
		=> Build(nameof(IFileVersionInfoFactory.GetVersionInfo),
			ParameterMatcher.From("fileName", fileName, fileNameExpression));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, quantifier,
					s => s.FileVersionInfo, BucketDescription, methodName, matchers)),
			_subject, quantifier);
	}
}
