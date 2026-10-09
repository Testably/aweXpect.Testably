using aweXpect.Core;
using aweXpect.Testably.Recorded;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably;

/// <summary>
///     Extension methods for asserting against <see cref="IFileSystemStatistics" />.
/// </summary>
public static class StatisticsExtensions
{
	/// <summary>
	///     Entry-point for assertions on recorded calls and property accesses.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Mirror methods consolidate every overload of the underlying interface method into a single signature
	///         whose optional <see cref="System.Func{T,TResult}" />-shaped predicates are matched <b>positionally</b>
	///         against the recorded parameters. Concretely:
	///     </para>
	///     <list type="bullet">
	///         <item>
	///             Supplying no predicate (or <see langword="null" />) for an argument skips that position and
	///             matches every overload — so an unfiltered call like <c>.File.Open()</c> counts <i>all</i> recorded
	///             <c>Open</c> invocations regardless of arity.
	///         </item>
	///         <item>
	///             Supplying a predicate whose <i>position</i> exceeds the recorded arity of a given overload
	///             excludes that overload — e.g. filtering <c>recursive</c> on <c>Directory.Delete</c> only matches
	///             the two-argument overload; filtering <c>path4</c> on <c>Path.Combine</c> only matches the
	///             four-argument overload.
	///         </item>
	///         <item>
	///             Supplying a predicate whose <i>type</i> differs from the type recorded at that position
	///             silently excludes that overload — e.g. filtering <c>searchOption</c> on
	///             <c>Directory.EnumerateDirectories</c> never matches the
	///             <c>EnumerationOptions</c>-bearing overload because <c>Is&lt;SearchOption&gt;</c> returns
	///             <see langword="false" /> when the recorded value is an <c>EnumerationOptions</c>.
	///         </item>
	///     </list>
	///     <para>
	///         A handful of methods cannot be filtered fully through this positional model because two overloads
	///         place different types at the same recording position (<c>File.Open</c> / <c>FileInfo.Open</c> with
	///         <c>FileStreamOptions</c>, <c>FileSystemWatcher.WaitForChanged</c> with <c>TimeSpan</c>). The
	///         affected mirror methods document the limitation in their own remarks.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code>
	/// await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
	/// await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "foo.txt").Once();
	/// await That(fileSystem.Statistics).Recorded().FileInfo["foo.txt"].IsReadOnly.Set().Once();
	/// </code>
	/// </example>
	[GuaranteesNotNull]
	public static RecordedFileSystemStatistics Recorded(this IThat<IFileSystemStatistics> subject)
		=> new(subject);
}
