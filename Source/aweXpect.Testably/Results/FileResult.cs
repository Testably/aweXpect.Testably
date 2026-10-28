using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public partial class FileResult<TType, TThat>
	: AndOrResult<TType, TThat>
	where TType : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TType, (IFileSystem fs, string fullPath)> _resolver;

	internal FileResult(
		ExpectationBuilder expectationBuilder,
		TThat subject,
		Func<TType, (IFileSystem fs, string fullPath)> resolver)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_resolver = resolver;
	}

	/// <summary>
	///     Verifies that the file content…
	/// </summary>
	public Content WithContent()
		=> new(_expectationBuilder, this, _resolver);

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> string content.
	/// </summary>
	public StringEqualityTypeResult<TType, FileResult<TType, TThat>> WithContent(
		string expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<TType, FileResult<TType, TThat>>(
			_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, expected), static (s, it, grammar)
				=> new HasStringContentEqualToConstraint(it, grammar, s.resolver, s.options, s.expected, nameof(expected))),
			this, options);
	}

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> binary content.
	/// </summary>
	public AndOrResult<TType, FileResult<TType, TThat>> WithContent(
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		return new AndOrResult<TType, FileResult<TType, TThat>>(
			_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, expected, doNotPopulateThisValue), static (s, it, grammars)
				=> new HasBinaryContentEqualToConstraint(it, grammars, s.resolver, s.expected, s.doNotPopulateThisValue)),
			this);
	}

	/// <summary>
	///     Verifies that the string content of the file satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TType, FileResult<TType, TThat>> WhoseContent(
		Action<IThatSubject<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));

		Func<TType, (IFileSystem fs, string fullPath)> resolver = _resolver;
		return new AndOrResult<TType, FileResult<TType, TThat>>(
			_expectationBuilder
				.ForMember(
					MemberAccessor<TType, string>.FromFuncWithName(
						p =>
						{
							(IFileSystem fs, string fullPath) = resolver(p);
							return fs.File.ReadAllText(fullPath);
						},
						"content "),
					(member, expectation) => expectation.Append(" whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<string?>(e))),
			this);
	}

	/// <summary>
	///     Verifies that the creation time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TType, FileResult<TType, TThat>> WithCreationTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).CreationTime, expected, "creation time");

	/// <summary>
	///     Verifies that the last access time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastAccessTime" /> or <see cref="IFileSystemInfo.LastAccessTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TType, FileResult<TType, TThat>> WithLastAccessTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).LastAccessTime, expected, "last access time");

	/// <summary>
	///     Verifies that the last write time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastWriteTime" /> or <see cref="IFileSystemInfo.LastWriteTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TType, FileResult<TType, TThat>> WithLastWriteTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).LastWriteTime, expected, "last write time");

	private TimeToleranceResult<TType, FileResult<TType, TThat>> AddTimeConstraint(
		Func<TType, DateTime> timeAccessor, DateTime expected, string expectedString)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<TType, FileResult<TType, TThat>>(
			_expectationBuilder.And(" ").AddConstraint((timeAccessor, resolver: _resolver, tolerance, expected, expectedString), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<TType>(it, grammars,
					s.timeAccessor, p =>
					{
						(IFileSystem fs, string fullPath) = s.resolver(p);
						return GetMissingFileResult(fs, fullPath);
					}, s.tolerance, s.expected, s.expectedString,
					isWithClause: true)),
			this, tolerance);
	}

	private IFileSystemInfo GetInfo(TType source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.FileInfo.New(fullPath);
	}
}
