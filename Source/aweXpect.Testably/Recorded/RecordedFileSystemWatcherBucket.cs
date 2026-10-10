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
///     Assertions on recorded calls against <see cref="IFileSystemWatcherFactory" /> (factory-level)
///     and per-instance assertions accessed via the path indexer.
/// </summary>
public sealed class RecordedFileSystemWatcherBucket
{
	private const string BucketDescription = "FileSystemWatcher";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileSystemWatcherBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded calls against the <see cref="IFileSystemWatcher" /> instance for <paramref name="path" />.
	/// </summary>
	public RecordedFileSystemWatcherInstance this[string path]
	{
		get
		{
			ThrowHelper.ThrowIfNull(path, nameof(path));

			return new RecordedFileSystemWatcherInstance(_subject, path);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemWatcherFactory.New()" /> and overloads.
	/// </summary>
	public RecordedMethodCallResult New(
		Func<string, bool>? path = null,
		Func<string, bool>? filter = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(filter))]
		string? filterExpression = null)
		=> Build(nameof(IFileSystemWatcherFactory.New),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("filter", filter, filterExpression));

	/// <summary>
	///     Recorded calls to <see cref="IFileSystemWatcherFactory.Wrap(FileSystemWatcher)" />.
	/// </summary>
	public RecordedMethodCallResult Wrap(
		Func<FileSystemWatcher, bool>? fileSystemWatcher = null,
		[CallerArgumentExpression(nameof(fileSystemWatcher))]
		string? fileSystemWatcherExpression = null)
		=> Build(nameof(IFileSystemWatcherFactory.Wrap),
			ParameterMatcher.From("fileSystemWatcher", fileSystemWatcher, fileSystemWatcherExpression));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, quantifier,
					s => s.FileSystemWatcher, BucketDescription, methodName, matchers)),
			_subject, quantifier);
	}
}
