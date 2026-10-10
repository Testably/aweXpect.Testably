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
///     Assertions on recorded calls against <see cref="IFileStreamFactory" /> (factory-level)
///     and per-instance assertions accessed via the path indexer.
/// </summary>
/// <remarks>
///     The SafeFileHandle-based <c>New</c> overloads are not mirrored. <c>FileStreamOptions</c> and the trailing
///     <c>FileOptions</c>/<c>useAsync</c> arguments cannot be filtered (they share parameter positions with
///     other types across overloads).
/// </remarks>
public sealed class RecordedFileStreamBucket
{
	private const string BucketDescription = "FileStream";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileStreamBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded calls against the <see cref="FileSystemStream" /> instance for <paramref name="path" />.
	/// </summary>
	public RecordedFileStreamInstance this[string path]
	{
		get
		{
			ThrowHelper.ThrowIfNull(path, nameof(path));

			return new RecordedFileStreamInstance(_subject, path);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IFileStreamFactory.New(string, FileMode)" /> and string-path overloads.
	/// </summary>
#pragma warning disable S107 // Each filter Func<> pairs with a CallerArgumentExpression string for diagnostics.
	public RecordedMethodCallResult New(
		Func<string, bool>? path = null,
		Func<FileMode, bool>? mode = null,
		Func<FileAccess, bool>? access = null,
		Func<FileShare, bool>? share = null,
		Func<int, bool>? bufferSize = null,
		[CallerArgumentExpression(nameof(path))]
		string? pathExpression = null,
		[CallerArgumentExpression(nameof(mode))]
		string? modeExpression = null,
		[CallerArgumentExpression(nameof(access))]
		string? accessExpression = null,
		[CallerArgumentExpression(nameof(share))]
		string? shareExpression = null,
		[CallerArgumentExpression(nameof(bufferSize))]
		string? bufferSizeExpression = null)
		=> Build(nameof(IFileStreamFactory.New),
			ParameterMatcher.From("path", path, pathExpression),
			ParameterMatcher.From("mode", mode, modeExpression),
			ParameterMatcher.From("access", access, accessExpression),
			ParameterMatcher.From("share", share, shareExpression),
			ParameterMatcher.From("bufferSize", bufferSize, bufferSizeExpression));
#pragma warning restore S107

	/// <summary>
	///     Recorded calls to <see cref="IFileStreamFactory.Wrap(FileStream)" />.
	/// </summary>
	public RecordedMethodCallResult Wrap(
		Func<FileStream, bool>? fileStream = null,
		[CallerArgumentExpression(nameof(fileStream))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IFileStreamFactory.Wrap),
			ParameterMatcher.From("fileStream", fileStream, doNotPopulateThisValue));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.FileStream, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
