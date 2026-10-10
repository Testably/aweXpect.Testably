using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded property accesses (getter or setter).
/// </summary>
public sealed class RecordedProperty
{
	private readonly string _bucketDescription;
	private readonly Func<IFileSystemStatistics, IStatistics> _bucketSelector;
	private readonly string _propertyName;
	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedProperty(
		IThat<IFileSystemStatistics> subject,
		Func<IFileSystemStatistics, IStatistics> bucketSelector,
		string bucketDescription,
		string propertyName)
	{
		_subject = subject;
		_bucketSelector = bucketSelector;
		_bucketDescription = bucketDescription;
		_propertyName = propertyName;
	}

	/// <summary>
	///     Recorded reads (<see cref="PropertyAccess.Get" />) of this property.
	/// </summary>
	public RecordedPropertyAccessResult Get() => Build(PropertyAccess.Get);

	/// <summary>
	///     Recorded writes (<see cref="PropertyAccess.Set" />) of this property.
	/// </summary>
	public RecordedPropertyAccessResult Set() => Build(PropertyAccess.Set);

	private RecordedPropertyAccessResult Build(PropertyAccess access)
	{
		Quantifier quantifier = new();
		Func<IFileSystemStatistics, IStatistics> bucketSelector = _bucketSelector;
		string bucketDescription = _bucketDescription;
		string propertyName = _propertyName;
		return new RecordedPropertyAccessResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, bucketSelector, bucketDescription, propertyName, access), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedPropertyAccessConstraint(
					it, grammars, state.quantifier,
					state.bucketSelector, state.bucketDescription, state.propertyName, state.access)),
			_subject, quantifier);
	}
}
