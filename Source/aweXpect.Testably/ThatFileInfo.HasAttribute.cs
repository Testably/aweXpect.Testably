using System;
using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> HasAttribute(this IThat<IFileInfo?> subject,
		FileAttributes expected)
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected file attributes must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IFileInfo>(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> DoesNotHaveAttribute(this IThat<IFileInfo?> subject,
		FileAttributes unexpected)
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected file attributes must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpected, it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IFileInfo>(it, grammars, unexpected).Invert()),
			subject);
	}
}
