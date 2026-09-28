using System.IO.Abstractions;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> is empty (no files and no subdirectories).
	/// </summary>
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> IsEmpty(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEmptyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> is not empty.
	/// </summary>
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> IsNotEmpty(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEmptyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsEmptyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IDirectoryInfo>(it, grammars),
			IValueConstraint<IDirectoryInfo>
	{
		public ConstraintResult IsMetBy(IDirectoryInfo actual)
		{
			Actual = actual;
			if (!actual.Exists)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = !actual.EnumerateFileSystemInfos().Any() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is empty");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was not");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not empty");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was");
			}
		}
	}
}
