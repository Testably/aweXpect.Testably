using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Testably;

public static partial class DriveInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> available free space in bytes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> HasAvailableFreeSpace(this IThat<IDriveInfo> source,
		long expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new HasAvailableFreeSpaceConstraint(it, grammars, expected)),
			source);

	private sealed class HasAvailableFreeSpaceConstraint(string it, ExpectationGrammars grammars, long expected)
		: ConstraintResult.WithNotNullValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		private long _actualAvailableFreeSpace;

		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualAvailableFreeSpace = actual.AvailableFreeSpace;
			Outcome = _actualAvailableFreeSpace == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has available free space ", "have available free space "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, _actualAvailableFreeSpace);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have available free space ", "do not have available free space "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
