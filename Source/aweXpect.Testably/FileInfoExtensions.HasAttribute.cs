using System;
using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> HasAttribute(this IThat<IFileInfo> source,
		FileAttributes expected)
	{
		if (expected == default)
		{
			throw new ArgumentException(
				"The expected file attributes must include at least one flag.", nameof(expected));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IFileInfo>(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> DoesNotHaveAttribute(this IThat<IFileInfo> source,
		FileAttributes unexpected)
	{
		if (unexpected == default)
		{
			throw new ArgumentException(
				"The unexpected file attributes must include at least one flag.", nameof(unexpected));
		}

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IFileInfo>(it, grammars, unexpected).Invert()),
			source);
	}
}
