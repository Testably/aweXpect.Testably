using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the last access time of the <see cref="IFileInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.LastAccessTime" /> or <see cref="IFileSystemInfo.LastAccessTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IFileInfo, IThat<IFileInfo?>> HasLastAccessTime(
		this IThat<IFileInfo?> subject, DateTime expected)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((tolerance, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IFileInfo>(it, grammars,
					f => f.LastAccessTime, FileSystemConstraints.GetMissingResult, s.tolerance, s.expected, "last access time",
					isWithClause: false)),
			subject, tolerance);
	}
}
