using System;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> has a drive with the given <paramref name="driveName" />.
	/// </summary>
	/// <remarks>
	///     The drive is resolved by case-insensitive name match against
	///     <see cref="IDriveInfoFactory.GetDrives" />. UNC drives (which do not appear in
	///     <c>GetDrives()</c>) are not supported by this assertion.
	/// </remarks>
	[GuaranteesNotNull]
	public static DriveResult<TFileSystem> HasDrive<TFileSystem>(
		this IThat<TFileSystem> subject, string driveName)
		where TFileSystem : class, IFileSystem
	{
		ThrowHelper.ThrowIfNull(driveName, nameof(driveName));

		Func<TFileSystem, IDriveInfo?> resolver = fs => ResolveDrive(fs, driveName);
		return new DriveResult<TFileSystem>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasDriveConstraint<TFileSystem>(it, grammars, driveName, resolver)),
			subject,
			resolver);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileSystem" /> does not have a drive with the given <paramref name="driveName" />.
	/// </summary>
	/// <remarks>
	///     The drive is resolved by case-insensitive name match against
	///     <see cref="IDriveInfoFactory.GetDrives" />. UNC drives (which do not appear in
	///     <c>GetDrives()</c>) are not supported by this assertion.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TFileSystem, IThat<TFileSystem>> DoesNotHaveDrive<TFileSystem>(
		this IThat<TFileSystem> subject, string driveName)
		where TFileSystem : class, IFileSystem
	{
		ThrowHelper.ThrowIfNull(driveName, nameof(driveName));

		Func<TFileSystem, IDriveInfo?> resolver = fs => ResolveDrive(fs, driveName);
		return new AndOrResult<TFileSystem, IThat<TFileSystem>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasDriveConstraint<TFileSystem>(it, grammars, driveName, resolver).Invert()),
			subject);
	}

	private static IDriveInfo? ResolveDrive(IFileSystem fileSystem, string driveName)
	{
		string normalized = NormalizeDriveName(driveName);
		return fileSystem.DriveInfo.GetDrives()
			.FirstOrDefault(d => string.Equals(
				NormalizeDriveName(d.Name), normalized, StringComparison.OrdinalIgnoreCase));
	}

	private static string NormalizeDriveName(string name)
		=> name.TrimEnd('\\', '/');

	private sealed class HasDriveConstraint<TFileSystem>(
		string it,
		ExpectationGrammars grammars,
		string driveName,
		Func<TFileSystem, IDriveInfo?> resolver)
		: ConstraintResult.WithNotNullValue<TFileSystem>(it, grammars),
			IValueConstraint<TFileSystem>
		where TFileSystem : class, IFileSystem
	{
		public ConstraintResult IsMetBy(TFileSystem actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			Outcome = resolver(actual) is not null ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has drive ", "have drive "));
			Formatter.Format(stringBuilder, driveName);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not exist");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have drive ", "do not have drive "));
			Formatter.Format(stringBuilder, driveName);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
