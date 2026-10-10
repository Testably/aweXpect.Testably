using System;
using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> has the <paramref name="expected" />
	///     <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> HasAttribute(this IThat<IDirectoryInfo> source,
		FileAttributes expected)
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected file attributes must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IDirectoryInfo>(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileAttributes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> DoesNotHaveAttribute(
		this IThat<IDirectoryInfo> source,
		FileAttributes unexpected)
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected file attributes must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasAttributeConstraint<IDirectoryInfo>(it, grammars, unexpected).Invert()),
			source);
	}
}
