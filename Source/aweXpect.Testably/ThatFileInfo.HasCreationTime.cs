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
	///     Verifies that the creation time of the <see cref="IFileInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IFileInfo, IThat<IFileInfo?>> HasCreationTime(
		this IThat<IFileInfo?> subject, DateTime expected)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((tolerance, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IFileInfo>(it, grammars,
					f => f.CreationTime, FileSystemConstraints.GetMissingResult, s.tolerance, s.expected, "creation time",
					isWithClause: false)),
			subject, tolerance);
	}
}
