using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a drive.
/// </summary>
public class DriveResult<TParent>
	: AndOrResult<TParent, IThat<TParent>>
	where TParent : class
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly Func<TParent, IDriveInfo?> _resolver;

	internal DriveResult(
		ExpectationBuilder expectationBuilder,
		IThat<TParent> subject,
		Func<TParent, IDriveInfo?> resolver)
		: base(expectationBuilder, subject)
	{
		_expectationBuilder = expectationBuilder;
		_resolver = resolver;
	}

	/// <summary>
	///     Further expectations on the <see cref="IDriveInfo" /> identified by this drive result.
	/// </summary>
	public IThat<IDriveInfo> Which
		=> new ThatSubject<IDriveInfo>(
			_expectationBuilder.ForWhich(
				_resolver, " which ", "the drive"));
}
