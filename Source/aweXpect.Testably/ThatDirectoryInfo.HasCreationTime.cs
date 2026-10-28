using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatDirectoryInfo
{
	/// <summary>
	///     Verifies that the creation time of the <see cref="IDirectoryInfo" /> matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Uses <see cref="IFileSystemInfo.CreationTime" /> or <see cref="IFileSystemInfo.CreationTimeUtc" /> depending
	///     on the <see cref="DateTime.Kind" /> property of the <paramref name="expected" /> value.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo?>> HasCreationTime(
		this IThat<IDirectoryInfo?> subject, DateTime expected)
	{
		OptionsHolder<TimeTolerance> tolerance = new(TimeTolerance.Default);
		return new TimeToleranceResult<IDirectoryInfo, IThat<IDirectoryInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((tolerance, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasTimeConstraint<IDirectoryInfo>(it, grammars,
					d => d.CreationTime, FileSystemConstraints.GetMissingResult, s.tolerance, s.expected, "creation time",
					isWithClause: false)),
			subject, tolerance);
	}
}
