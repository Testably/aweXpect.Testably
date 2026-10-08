using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result of a recorded property access assertion.
/// </summary>
/// <remarks>
///     Supports the count vocabulary (<c>Once</c>, <c>Twice</c>, <c>Never</c>, <c>Exactly</c>,
///     <c>AtLeast</c>, <c>AtMost</c>, …) from <see cref="QuantifierExtensions" />.
/// </remarks>
public sealed class RecordedPropertyAccessResult
	: AndOrResult<IFileSystemStatistics, IThat<IFileSystemStatistics>, RecordedPropertyAccessResult>,
		IOptionsProvider<Quantifier>
{
	private readonly Quantifier _quantifier;

	internal RecordedPropertyAccessResult(
		ExpectationBuilder expectationBuilder,
		IThat<IFileSystemStatistics> subject,
		Quantifier quantifier)
		: base(expectationBuilder, subject)
	{
		_quantifier = quantifier;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;
}
