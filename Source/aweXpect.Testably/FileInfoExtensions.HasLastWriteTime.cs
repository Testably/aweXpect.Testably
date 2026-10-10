using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the last write time of the <see cref="IFileInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastWriteTime" /> or <see cref="IFileSystemInfo.LastWriteTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IFileInfo, IThat<IFileInfo>> HasLastWriteTime(
		this IThat<IFileInfo> source, DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((tolerance, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IFileInfo>(it, grammars,
					f => f.LastWriteTime, FileSystemConstraints.GetMissingResult, s.tolerance, s.expected, "last write time",
					isWithClause: false)),
			source, tolerance);
	}
}
