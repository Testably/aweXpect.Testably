using System.IO;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Testably;

public static partial class DriveInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> <see cref="DriveType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> HasDriveType(this IThat<IDriveInfo> source,
		DriveType expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new HasDriveTypeConstraint(it, grammars, expected)),
			source);

	private sealed class HasDriveTypeConstraint(string it, ExpectationGrammars grammars, DriveType expected)
		: ConstraintResult.WithNotNullValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		private DriveType _actualDriveType;

		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualDriveType = actual.DriveType;
			Outcome = _actualDriveType == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has drive type ", "have drive type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, _actualDriveType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have drive type ", "do not have drive type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
