using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> has a file at the given <paramref name="path" />
	///     (relative to the directory).
	/// </summary>
	[GuaranteesNotNull]
	public static FileResult<IDirectoryInfo> HasFile(
		this IThat<IDirectoryInfo> subject, string path)
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<IDirectoryInfo, (IFileSystem fs, string fullPath)> resolver =
			d => (d.FileSystem, d.FileSystem.Path.Combine(d.FullName, path));
		return new FileResult<IDirectoryInfo>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasFileConstraint<IDirectoryInfo>(it, grammars, path, resolver)),
			subject,
			resolver);
	}

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not have a file at the given <paramref name="path" />
	///     (relative to the directory).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> DoesNotHaveFile(
		this IThat<IDirectoryInfo> subject, string path)
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<IDirectoryInfo, (IFileSystem fs, string fullPath)> resolver =
			d => (d.FileSystem, d.FileSystem.Path.Combine(d.FullName, path));
		return new AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasFileConstraint<IDirectoryInfo>(it, grammars, path, resolver).Invert()),
			subject);
	}
}
