using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against a single <see cref="FileSystemStream" /> instance (identified by path).
/// </summary>
/// <remarks>
///     Span/Memory overloads (e.g. <c>Read(Span&lt;byte&gt;)</c>) record their buffer as a span and cannot be
///     filtered through this mirror; their calls are still counted in unfiltered method assertions.
/// </remarks>
public sealed class RecordedFileStreamInstance
{
	private readonly string _bucketDescription;
	private readonly string _path;
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedFileStreamInstance(IThat<IFileSystemStatistics> subject, string path)
	{
		_subject = subject;
		_path = path;
		_bucketDescription = $"FileStream[\"{path}\"]";
	}

	/// <summary>Recorded accesses to <see cref="Stream.CanRead" />.</summary>
	public RecordedProperty CanRead => Property(nameof(Stream.CanRead));

	/// <summary>Recorded accesses to <see cref="Stream.CanSeek" />.</summary>
	public RecordedProperty CanSeek => Property(nameof(Stream.CanSeek));

	/// <summary>Recorded accesses to <see cref="Stream.CanTimeout" />.</summary>
	public RecordedProperty CanTimeout => Property(nameof(Stream.CanTimeout));

	/// <summary>Recorded accesses to <see cref="Stream.CanWrite" />.</summary>
	public RecordedProperty CanWrite => Property(nameof(Stream.CanWrite));

	/// <summary>Recorded accesses to <see cref="FileSystemStream.IsAsync" />.</summary>
	public RecordedProperty IsAsync => Property(nameof(FileSystemStream.IsAsync));

	/// <summary>Recorded accesses to <see cref="Stream.Length" />.</summary>
	public RecordedProperty Length => Property(nameof(Stream.Length));

	/// <summary>Recorded accesses to <see cref="FileSystemStream.Name" />.</summary>
	public RecordedProperty Name => Property(nameof(FileSystemStream.Name));

	/// <summary>Recorded accesses to <see cref="Stream.Position" />.</summary>
	public RecordedProperty Position => Property(nameof(Stream.Position));

	/// <summary>Recorded accesses to <see cref="Stream.ReadTimeout" />.</summary>
	public RecordedProperty ReadTimeout => Property(nameof(Stream.ReadTimeout));

	/// <summary>Recorded accesses to <see cref="Stream.WriteTimeout" />.</summary>
	public RecordedProperty WriteTimeout => Property(nameof(Stream.WriteTimeout));

	/// <summary>Recorded calls to <see cref="Stream.Close" />.</summary>
	public RecordedMethodCallResult Close()
		=> Build(nameof(Stream.Close));

	/// <summary>Recorded calls to <see cref="Stream.CopyTo(Stream)" /> and overloads.</summary>
	public RecordedMethodCallResult CopyTo(
		Func<Stream, bool>? destination = null,
		Func<int, bool>? bufferSize = null,
		[CallerArgumentExpression(nameof(destination))]
		string? destinationExpression = null,
		[CallerArgumentExpression(nameof(bufferSize))]
		string? bufferSizeExpression = null)
		=> Build(nameof(Stream.CopyTo),
			ParameterMatcher.From("destination", destination, destinationExpression),
			ParameterMatcher.From("bufferSize", bufferSize, bufferSizeExpression));

	/// <summary>Recorded calls to <see cref="Stream.CopyToAsync(Stream)" /> and overloads.</summary>
	public RecordedMethodCallResult CopyToAsync(
		Func<Stream, bool>? destination = null,
		Func<int, bool>? bufferSize = null,
		[CallerArgumentExpression(nameof(destination))]
		string? destinationExpression = null,
		[CallerArgumentExpression(nameof(bufferSize))]
		string? bufferSizeExpression = null)
		=> Build(nameof(Stream.CopyToAsync),
			ParameterMatcher.From("destination", destination, destinationExpression),
			ParameterMatcher.From("bufferSize", bufferSize, bufferSizeExpression));

	/// <summary>Recorded calls to <see cref="Stream.Flush" /> and <see cref="FileStream.Flush(bool)" />.</summary>
	public RecordedMethodCallResult Flush(
		Func<bool, bool>? flushToDisk = null,
		[CallerArgumentExpression(nameof(flushToDisk))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(Stream.Flush),
			ParameterMatcher.From("flushToDisk", flushToDisk, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="Stream.FlushAsync()" /> and overloads.</summary>
	public RecordedMethodCallResult FlushAsync(
		Func<CancellationToken, bool>? cancellationToken = null,
		[CallerArgumentExpression(nameof(cancellationToken))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(Stream.FlushAsync),
			ParameterMatcher.From("cancellationToken", cancellationToken, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="Stream.Read(byte[], int, int)" />.</summary>
	public RecordedMethodCallResult Read(
		Func<byte[], bool>? buffer = null,
		Func<int, bool>? offset = null,
		Func<int, bool>? count = null,
		[CallerArgumentExpression(nameof(buffer))]
		string? bufferExpression = null,
		[CallerArgumentExpression(nameof(offset))]
		string? offsetExpression = null,
		[CallerArgumentExpression(nameof(count))]
		string? countExpression = null)
		=> Build(nameof(Stream.Read),
			ParameterMatcher.From("buffer", buffer, bufferExpression),
			ParameterMatcher.From("offset", offset, offsetExpression),
			ParameterMatcher.From("count", count, countExpression));

	/// <summary>Recorded calls to <see cref="Stream.ReadAsync(byte[], int, int)" /> and overloads.</summary>
	public RecordedMethodCallResult ReadAsync(
		Func<byte[], bool>? buffer = null,
		Func<int, bool>? offset = null,
		Func<int, bool>? count = null,
		[CallerArgumentExpression(nameof(buffer))]
		string? bufferExpression = null,
		[CallerArgumentExpression(nameof(offset))]
		string? offsetExpression = null,
		[CallerArgumentExpression(nameof(count))]
		string? countExpression = null)
		=> Build(nameof(Stream.ReadAsync),
			ParameterMatcher.From("buffer", buffer, bufferExpression),
			ParameterMatcher.From("offset", offset, offsetExpression),
			ParameterMatcher.From("count", count, countExpression));

	/// <summary>Recorded calls to <see cref="Stream.ReadByte" />.</summary>
	public RecordedMethodCallResult ReadByte()
		=> Build(nameof(Stream.ReadByte));

	/// <summary>Recorded calls to <see cref="Stream.Seek(long, SeekOrigin)" />.</summary>
	public RecordedMethodCallResult Seek(
		Func<long, bool>? offset = null,
		Func<SeekOrigin, bool>? origin = null,
		[CallerArgumentExpression(nameof(offset))]
		string? offsetExpression = null,
		[CallerArgumentExpression(nameof(origin))]
		string? originExpression = null)
		=> Build(nameof(Stream.Seek),
			ParameterMatcher.From("offset", offset, offsetExpression),
			ParameterMatcher.From("origin", origin, originExpression));

	/// <summary>Recorded calls to <see cref="Stream.SetLength(long)" />.</summary>
	public RecordedMethodCallResult SetLength(
		Func<long, bool>? value = null,
		[CallerArgumentExpression(nameof(value))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(Stream.SetLength),
			ParameterMatcher.From("value", value, doNotPopulateThisValue));

	/// <summary>Recorded calls to <see cref="Stream.Write(byte[], int, int)" />.</summary>
	public RecordedMethodCallResult Write(
		Func<byte[], bool>? buffer = null,
		Func<int, bool>? offset = null,
		Func<int, bool>? count = null,
		[CallerArgumentExpression(nameof(buffer))]
		string? bufferExpression = null,
		[CallerArgumentExpression(nameof(offset))]
		string? offsetExpression = null,
		[CallerArgumentExpression(nameof(count))]
		string? countExpression = null)
		=> Build(nameof(Stream.Write),
			ParameterMatcher.From("buffer", buffer, bufferExpression),
			ParameterMatcher.From("offset", offset, offsetExpression),
			ParameterMatcher.From("count", count, countExpression));

	/// <summary>Recorded calls to <see cref="Stream.WriteAsync(byte[], int, int)" /> and overloads.</summary>
	public RecordedMethodCallResult WriteAsync(
		Func<byte[], bool>? buffer = null,
		Func<int, bool>? offset = null,
		Func<int, bool>? count = null,
		[CallerArgumentExpression(nameof(buffer))]
		string? bufferExpression = null,
		[CallerArgumentExpression(nameof(offset))]
		string? offsetExpression = null,
		[CallerArgumentExpression(nameof(count))]
		string? countExpression = null)
		=> Build(nameof(Stream.WriteAsync),
			ParameterMatcher.From("buffer", buffer, bufferExpression),
			ParameterMatcher.From("offset", offset, offsetExpression),
			ParameterMatcher.From("count", count, countExpression));

	/// <summary>Recorded calls to <see cref="Stream.WriteByte(byte)" />.</summary>
	public RecordedMethodCallResult WriteByte(
		Func<byte, bool>? value = null,
		[CallerArgumentExpression(nameof(value))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(Stream.WriteByte),
			ParameterMatcher.From("value", value, doNotPopulateThisValue));

	private RecordedProperty Property(string propertyName)
	{
		string path = _path;
		return new RecordedProperty(_subject, s => s.FileStream[path], _bucketDescription, propertyName);
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
					s => s.FileStream[state.path], state.bucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
