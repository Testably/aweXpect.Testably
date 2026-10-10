using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a directory.
/// </summary>
public partial class DirectoryResult<TParent>
	: AndOrResult<TParent, IThat<TParent>>
	where TParent : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TParent, (IFileSystem fs, string fullPath)> _resolver;

	internal DirectoryResult(
		ExpectationBuilder expectationBuilder,
		IThat<TParent> subject,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_resolver = resolver;
	}

	/// <summary>
	///     Verifies that the files of the directory satisfy the <paramref name="expectations" />.
	/// </summary>
	public DirectoryResult<TParent> WithFiles(
		Action<IThat<IEnumerable<IFileInfo>>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));

		Func<TParent, (IFileSystem fs, string fullPath)> resolver = _resolver;
		_expectationBuilder
			.ForMember(
				MemberAccessor<TParent, IEnumerable<IFileInfo>>.FromFunc(
					p =>
					{
						(IFileSystem fs, string fullPath) = resolver(p);
						return fs.Directory.EnumerateFiles(fullPath).Select(x => fs.FileInfo.New(x));
					},
					"files "),
				(member, expectation) => expectation.Append(" whose ").Append(member))
			.AddExpectations(
				e => expectations(new ThatSubject<IEnumerable<IFileInfo>>(e)),
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}

	/// <summary>
	///     Verifies that the subdirectories of the directory satisfy the <paramref name="expectations" />.
	/// </summary>
	public DirectoryResult<TParent> WithDirectories(
		Action<IThat<IEnumerable<IDirectoryInfo>>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));

		Func<TParent, (IFileSystem fs, string fullPath)> resolver = _resolver;
		_expectationBuilder
			.ForMember(
				MemberAccessor<TParent, IEnumerable<IDirectoryInfo>>.FromFunc(
					p =>
					{
						(IFileSystem fs, string fullPath) = resolver(p);
						return fs.Directory.EnumerateDirectories(fullPath).Select(x => fs.DirectoryInfo.New(x));
					},
					"subdirectories "),
				(member, expectation) => expectation.Append(" whose ").Append(member))
			.AddExpectations(
				e => expectations(new ThatSubject<IEnumerable<IDirectoryInfo>>(e)),
				grammars => grammars | ExpectationGrammars.Nested | ExpectationGrammars.Plural);
		return this;
	}
}
