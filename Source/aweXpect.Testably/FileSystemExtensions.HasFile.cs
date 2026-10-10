using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> has a file at the given <paramref name="path" />.
	/// </summary>
	[GuaranteesNotNull]
	public static FileResult<TFileSystem> HasFile<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : class, IFileSystem
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<TFileSystem, (IFileSystem fs, string fullPath)> resolver = fs => (fs, path);
		return new FileResult<TFileSystem>(
			subject.Get().ExpectationBuilder.AddConstraint((path, resolver), static (s, it, grammars)
				=> new FileSystemConstraints.HasFileConstraint<TFileSystem>(it, grammars, s.path, s.resolver)),
			subject,
			resolver);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> does not have a file at the given <paramref name="path" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TFileSystem, IThat<TFileSystem>> DoesNotHaveFile<TFileSystem>(
		this IThat<TFileSystem> subject, string path)
		where TFileSystem : class, IFileSystem
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<TFileSystem, (IFileSystem fs, string fullPath)> resolver = fs => (fs, path);
		return new AndOrResult<TFileSystem, IThat<TFileSystem>>(
			subject.Get().ExpectationBuilder.AddConstraint((path, resolver), static (s, it, grammars)
				=> new FileSystemConstraints.HasFileConstraint<TFileSystem>(it, grammars, s.path, s.resolver).Invert()),
			subject);
	}
}
