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
///     Assertions on recorded calls against a single <see cref="IFileSystemWatcher" /> instance (identified by path).
/// </summary>
public sealed class RecordedFileSystemWatcherInstance
{
	private readonly string _bucketDescription;
	private readonly string _path;
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileSystemWatcherInstance(IThat<IFileSystemStatistics> subject, string path)
	{
		_subject = subject;
		_path = path;
		_bucketDescription = $"FileSystemWatcher[\"{path}\"]";
	}

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.EnableRaisingEvents" />.</summary>
	public RecordedProperty EnableRaisingEvents => Property(nameof(IFileSystemWatcher.EnableRaisingEvents));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.Filter" />.</summary>
	public RecordedProperty Filter => Property(nameof(IFileSystemWatcher.Filter));

	/// <summary>Recorded accesses to <c>IFileSystemWatcher.Filters</c>.</summary>
#if NET6_0_OR_GREATER
	public RecordedProperty Filters => Property(nameof(IFileSystemWatcher.Filters));
#else
	public RecordedProperty Filters => Property("Filters");
#endif

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.IncludeSubdirectories" />.</summary>
	public RecordedProperty IncludeSubdirectories => Property(nameof(IFileSystemWatcher.IncludeSubdirectories));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.InternalBufferSize" />.</summary>
	public RecordedProperty InternalBufferSize => Property(nameof(IFileSystemWatcher.InternalBufferSize));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.NotifyFilter" />.</summary>
	public RecordedProperty NotifyFilter => Property(nameof(IFileSystemWatcher.NotifyFilter));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.Path" />.</summary>
	public RecordedProperty Path => Property(nameof(IFileSystemWatcher.Path));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.Site" />.</summary>
	public RecordedProperty Site => Property(nameof(IFileSystemWatcher.Site));

	/// <summary>Recorded accesses to <see cref="IFileSystemWatcher.SynchronizingObject" />.</summary>
	public RecordedProperty SynchronizingObject => Property(nameof(IFileSystemWatcher.SynchronizingObject));

	/// <summary>Recorded calls to <see cref="IFileSystemWatcher.BeginInit" />.</summary>
	public RecordedMethodCallResult BeginInit()
		=> Build(nameof(IFileSystemWatcher.BeginInit));

	/// <summary>Recorded calls to <see cref="IFileSystemWatcher.EndInit" />.</summary>
	public RecordedMethodCallResult EndInit()
		=> Build(nameof(IFileSystemWatcher.EndInit));

	/// <summary>Recorded calls to <see cref="IFileSystemWatcher.WaitForChanged(WatcherChangeTypes)" /> and overloads.</summary>
	/// <remarks>
	///     The <c>TimeSpan</c> overload records a <see cref="TimeSpan" /> at parameter position 1 while
	///     <paramref name="timeout" /> expects an <see cref="int" />; filtering by <paramref name="timeout" />
	///     only matches the <c>WaitForChanged(WatcherChangeTypes, int)</c> overload.
	/// </remarks>
	public RecordedMethodCallResult WaitForChanged(
		Func<WatcherChangeTypes, bool>? changeType = null,
		Func<int, bool>? timeout = null,
		[CallerArgumentExpression(nameof(changeType))]
		string? changeTypeExpression = null,
		[CallerArgumentExpression(nameof(timeout))]
		string? timeoutExpression = null)
		=> Build(nameof(IFileSystemWatcher.WaitForChanged),
			ParameterMatcher.From("changeType", changeType, changeTypeExpression),
			ParameterMatcher.From("timeout", timeout, timeoutExpression));

	private RecordedProperty Property(string propertyName)
	{
		string path = _path;
		return new RecordedProperty(_subject, s => s.FileSystemWatcher[path], _bucketDescription, propertyName);
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
					s => s.FileSystemWatcher[state.path], state.bucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
