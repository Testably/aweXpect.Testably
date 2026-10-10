using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the last access time of the <see cref="IDirectoryInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastAccessTime" /> or <see cref="IFileSystemInfo.LastAccessTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>> HasLastAccessTime(
		this IThat<IDirectoryInfo> source, DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((tolerance, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IDirectoryInfo>(it, grammars,
					d => d.LastAccessTime, FileSystemConstraints.GetMissingResult, s.tolerance, s.expected, "last access time",
					isWithClause: false)),
			source, tolerance);
	}
}
