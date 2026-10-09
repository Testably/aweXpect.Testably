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
	///     Verifies that the creation time of the <see cref="IFileInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IFileInfo, IThat<IFileInfo>> HasCreationTime(
		this IThat<IFileInfo> source, DateTime expected)
	{
		TimeTolerance tolerance = new();
		return new TimeToleranceResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IFileInfo>(it, grammars,
					f => f.CreationTime, FileSystemConstraints.GetMissingResult, tolerance, expected, "creation time",
					isWithClause: false)),
			source, tolerance);
	}
}
