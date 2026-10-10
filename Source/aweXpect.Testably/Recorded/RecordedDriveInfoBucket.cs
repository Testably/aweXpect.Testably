using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Recorded;

/// <summary>
///     Assertions on recorded calls against <see cref="IDriveInfoFactory" /> (factory-level)
///     and per-instance assertions accessed via the drive-name indexer.
/// </summary>
public sealed class RecordedDriveInfoBucket
{
	private const string BucketDescription = "DriveInfo";

	private readonly IThat<IFileSystemStatistics> _subject;

	internal RecordedDriveInfoBucket(IThat<IFileSystemStatistics> subject)
	{
		_subject = subject;
	}

	/// <summary>
	///     Assertions on recorded calls against the <see cref="IDriveInfo" /> instance for <paramref name="driveName" />.
	/// </summary>
	public RecordedDriveInfoInstance this[string driveName]
	{
		get
		{
			ThrowHelper.ThrowIfNull(driveName, nameof(driveName));

			return new RecordedDriveInfoInstance(_subject, driveName);
		}
	}

	/// <summary>
	///     Recorded calls to <see cref="IDriveInfoFactory.GetDrives" />.
	/// </summary>
	public RecordedMethodCallResult GetDrives()
		=> Build(nameof(IDriveInfoFactory.GetDrives));

	/// <summary>
	///     Recorded calls to <see cref="IDriveInfoFactory.New(string)" />.
	/// </summary>
	public RecordedMethodCallResult New(
		Func<string, bool>? driveName = null,
		[CallerArgumentExpression(nameof(driveName))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDriveInfoFactory.New),
			ParameterMatcher.From("driveName", driveName, doNotPopulateThisValue));

	/// <summary>
	///     Recorded calls to <see cref="IDriveInfoFactory.Wrap(System.IO.DriveInfo)" />.
	/// </summary>
	public RecordedMethodCallResult Wrap(
		Func<DriveInfo, bool>? driveInfo = null,
		[CallerArgumentExpression(nameof(driveInfo))]
		string? doNotPopulateThisValue = null)
		=> Build(nameof(IDriveInfoFactory.Wrap),
			ParameterMatcher.From("driveInfo", driveInfo, doNotPopulateThisValue));

	private RecordedMethodCallResult Build(string methodName, params ParameterMatcher[] matchers)
	{
		Quantifier quantifier = new();
		return new RecordedMethodCallResult(
			_subject.Get().ExpectationBuilder.AddConstraint((quantifier, methodName, matchers), static (state, it, grammars)
				=> new StatisticsConstraints.RecordedMethodCallConstraint(
					it, grammars, state.quantifier,
					s => s.DriveInfo, BucketDescription, state.methodName, state.matchers)),
			_subject, quantifier);
	}
}
