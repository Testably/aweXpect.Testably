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
	///     Verifies that the <see cref="IDirectoryInfo" /> has a directory at the given <paramref name="path" />
	///     (relative to the directory).
	/// </summary>
	[GuaranteesNotNull]
	public static DirectoryResult<IDirectoryInfo> HasDirectory(
		this IThat<IDirectoryInfo> subject, string path)
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<IDirectoryInfo, (IFileSystem fs, string fullPath)> resolver =
			d => (d.FileSystem, d.FileSystem.Path.Combine(d.FullName, path));
		return new DirectoryResult<IDirectoryInfo>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasDirectoryConstraint<IDirectoryInfo>(it, grammars, path, resolver)),
			subject,
			resolver);
	}

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not have a directory at the given <paramref name="path" />
	///     (relative to the directory).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> DoesNotHaveDirectory(
		this IThat<IDirectoryInfo> subject, string path)
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));

		Func<IDirectoryInfo, (IFileSystem fs, string fullPath)> resolver =
			d => (d.FileSystem, d.FileSystem.Path.Combine(d.FullName, path));
		return new AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasDirectoryConstraint<IDirectoryInfo>(it, grammars, path, resolver).Invert()),
			subject);
	}
}
