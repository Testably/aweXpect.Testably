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
public partial class FileResult<TParent>
	: AndOrResult<TParent, IThat<TParent>>
	where TParent : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TParent, (IFileSystem fs, string fullPath)> _resolver;

	internal FileResult(
		ExpectationBuilder expectationBuilder,
		IThat<TParent> subject,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
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
	public StringEqualityTypeResult<TParent, FileResult<TParent>> WithContent(
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, expected), static (s, it, grammar)
				=> new HasStringContentEqualToConstraint(it, grammar, s.resolver, s.options, s.expected)),
			this, options);
	}

	/// <summary>
	///     Verifies that the file has the <paramref name="expected" /> binary content.
	/// </summary>
	public AndOrResult<TParent, FileResult<TParent>> WithContent(
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		return new AndOrResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, expected, doNotPopulateThisValue), static (s, it, grammars)
				=> new HasBinaryContentEqualToConstraint(it, grammars, s.resolver, s.expected, s.doNotPopulateThisValue)),
			this);
	}

	/// <summary>
	///     Verifies that the string content of the file satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TParent, FileResult<TParent>> WhoseContent(
		Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));

		Func<TParent, (IFileSystem fs, string fullPath)> resolver = _resolver;
		return new AndOrResult<TParent, FileResult<TParent>>(
			_expectationBuilder
				.ForMember(
					MemberAccessor<TParent, string>.FromFunc(
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
	public TimeToleranceResult<TParent, FileResult<TParent>> WithCreationTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).CreationTime, expected, "creation time");

	/// <summary>
	///     Verifies that the last access time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastAccessTime" /> or <see cref="IFileSystemInfo.LastAccessTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TParent, FileResult<TParent>> WithLastAccessTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).LastAccessTime, expected, "last access time");

	/// <summary>
	///     Verifies that the last write time of the file matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastWriteTime" /> or <see cref="IFileSystemInfo.LastWriteTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	public TimeToleranceResult<TParent, FileResult<TParent>> WithLastWriteTime(
		DateTime expected)
		=> AddTimeConstraint(p => GetInfo(p).LastWriteTime, expected, "last write time");

	private TimeToleranceResult<TParent, FileResult<TParent>> AddTimeConstraint(
		Func<TParent, DateTime> timeAccessor, DateTime expected, string expectedString)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<TParent, FileResult<TParent>>(
			_expectationBuilder.And(" ").AddConstraint((timeAccessor, resolver: _resolver, tolerance, expected, expectedString), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<TParent>(it, grammars,
					s.timeAccessor, p =>
					{
						(IFileSystem fs, string fullPath) = s.resolver(p);
						return GetMissingFileResult(fs, fullPath);
					}, s.tolerance, s.expected, s.expectedString,
					isWithClause: true)),
			this, tolerance);
	}

	private IFileSystemInfo GetInfo(TParent source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.FileInfo.New(fullPath);
	}
}
