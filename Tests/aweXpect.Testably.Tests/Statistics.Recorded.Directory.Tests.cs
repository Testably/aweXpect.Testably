using System.IO;
using System.Linq;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Directory
		{
			public sealed class CreateDirectoryTests
			{
				[Fact]
				public async Task CreateDirectory_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateDirectory with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldMatchOnce()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateDirectory().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldRejectNonMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(p => p == "other").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateDirectory with path matching p => p == "other" exactly once,
						             but it was recorded 0 times
						             """);
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task WithUnixCreateModeFilter_ShouldOnlyMatchTwoArgOverload()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.Directory.CreateDirectory("a");
#pragma warning disable CA1416
					fileSystem.Directory.CreateDirectory("b", UnixFileMode.UserRead);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(unixCreateMode: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task CreateDirectory_WithUnixCreateModeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateDirectory(unixCreateMode: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateDirectory with unixCreateMode matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif
			}

			public sealed class DeleteTests
			{
				[Fact]
				public async Task Delete_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Delete(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.Delete with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Delete_WithRecursiveFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Delete(recursive: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.Delete with recursive matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WithAtLeastOnce_ShouldSucceedAfterTwoCalls()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					fileSystem.Directory.Delete("a");
					fileSystem.Directory.Delete("b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.Delete().AtLeast().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerateDirectoriesTests
			{
				[Fact]
				public async Task EnumerateDirectories_FilteringSearchOption_ShouldOnlyMatchThreeArg()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateDirectories("d").ToList();
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", SearchOption.AllDirectories).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task EnumerateDirectories_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateDirectories with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateDirectories_WithSearchOptionFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateDirectories with searchOption matching o => o == SearchOption.AllDirectories exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateDirectories_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateDirectories with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task FilteringEnumerationOptions_ShouldOnlyMatchEnumerationOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", SearchOption.AllDirectories).ToList();
					_ = fileSystem.Directory.EnumerateDirectories("d", "*", new EnumerationOptions()).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(enumerationOptions: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task EnumerateDirectories_WithEnumerationOptionsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateDirectories(enumerationOptions: o => o != null).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateDirectories with enumerationOptions matching o => o != null exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif
			}

			public sealed class EnumerateFilesTests
			{
				[Fact]
				public async Task EnumerateFiles_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFiles(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFiles with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateFiles_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFiles(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFiles with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateFiles_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateFiles("d").ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFiles().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFilesTests
			{
				[Fact]
				public async Task GetFiles_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetFiles("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(p => p == "d").Once();
					}

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task GetFiles_WithEnumerationOptionsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(enumerationOptions: o => o != null).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFiles with enumerationOptions matching o => o != null exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif

				[Fact]
				public async Task GetFiles_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFiles with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetFiles_WithSearchOptionFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFiles with searchOption matching o => o == SearchOption.AllDirectories exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetFiles_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFiles(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFiles with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class EnumerateFileSystemEntriesTests
			{
#if NET8_0_OR_GREATER
				[Fact]
				public async Task EnumerateFileSystemEntries_WithEnumerationOptionsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries(enumerationOptions: o => o != null).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFileSystemEntries with enumerationOptions matching o => o != null exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif

				[Fact]
				public async Task EnumerateFileSystemEntries_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFileSystemEntries with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateFileSystemEntries_WithSearchOptionFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFileSystemEntries with searchOption matching o => o == SearchOption.AllDirectories exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EnumerateFileSystemEntries_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.EnumerateFileSystemEntries with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateFileSystemEntries("d").ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithSearchOptionFilter_ShouldOnlyMatchThreeArg()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.EnumerateFileSystemEntries("d").ToList();
					_ = fileSystem.Directory.EnumerateFileSystemEntries("d", "*", SearchOption.AllDirectories).ToList();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.EnumerateFileSystemEntries(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ExistsTests
			{
				[Fact]
				public async Task Exists_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Exists(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.Exists with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.Exists("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Exists(p => p == "foo").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetCreationTimeTests
			{
				[Fact]
				public async Task GetCreationTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetCreationTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetCreationTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetCreationTime("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetCreationTime().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetCreationTimeUtcTests
			{
				[Fact]
				public async Task GetCreationTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetCreationTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetCreationTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetCreationTimeUtc("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetCreationTimeUtc().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetDirectoriesTests
			{
#if NET8_0_OR_GREATER
				[Fact]
				public async Task GetDirectories_WithEnumerationOptionsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectories(enumerationOptions: o => o != null).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectories with enumerationOptions matching o => o != null exactly once,
						             but it was recorded 0 times
						             """);
				}
#endif

				[Fact]
				public async Task GetDirectories_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectories(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectories with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetDirectories_WithSearchOptionFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectories(searchOption: o => o == SearchOption.AllDirectories).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectories with searchOption matching o => o == SearchOption.AllDirectories exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetDirectories_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectories(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectories with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WithPathFilter_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetDirectories("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectories(p => p == "d").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetDirectoryRootTests
			{
				[Fact]
				public async Task GetDirectoryRoot_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectoryRoot(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectoryRoot with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetDirectoryRoot_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectoryRoot().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetDirectoryRoot exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.GetDirectoryRoot("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetDirectoryRoot().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFileSystemEntriesTests
			{
				[Fact]
				public async Task GetFileSystemEntries_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFileSystemEntries(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFileSystemEntries with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetFileSystemEntries_WithSearchPatternFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFileSystemEntries(searchPattern: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetFileSystemEntries with searchPattern matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetFileSystemEntries("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetFileSystemEntries().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetLastAccessTimeTests
			{
				[Fact]
				public async Task GetLastAccessTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastAccessTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetLastAccessTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetLastAccessTime("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastAccessTime().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetLastAccessTimeUtcTests
			{
				[Fact]
				public async Task GetLastAccessTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastAccessTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetLastAccessTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("d");
					_ = fileSystem.Directory.GetLastAccessTimeUtc("d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastAccessTimeUtc().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetLastWriteTimeTests
			{
				[Fact]
				public async Task GetLastWriteTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastWriteTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetLastWriteTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class GetLastWriteTimeUtcTests
			{
				[Fact]
				public async Task GetLastWriteTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetLastWriteTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetLastWriteTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class GetParentTests
			{
				[Fact]
				public async Task GetParent_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.GetParent(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.GetParent with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class MoveTests
			{
				[Fact]
				public async Task Move_WithDestDirNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Move(destDirName: p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.Move with destDirName matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Move_WithSourceDirNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.Move(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.Move with sourceDirName matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetCreationTimeTests
			{
				[Fact]
				public async Task SetCreationTime_WithCreationTimeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetCreationTime(creationTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetCreationTime with creationTime matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetCreationTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetCreationTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetCreationTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetCreationTimeUtcTests
			{
				[Fact]
				public async Task SetCreationTimeUtc_WithCreationTimeUtcFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetCreationTimeUtc(creationTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetCreationTimeUtc with creationTimeUtc matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetCreationTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetCreationTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetCreationTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetCurrentDirectoryTests
			{
				[Fact]
				public async Task SetCurrentDirectory_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetCurrentDirectory(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetCurrentDirectory with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetLastAccessTimeTests
			{
				[Fact]
				public async Task SetLastAccessTime_WithLastAccessTimeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastAccessTime(lastAccessTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastAccessTime with lastAccessTime matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetLastAccessTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastAccessTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastAccessTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetLastAccessTimeUtcTests
			{
				[Fact]
				public async Task SetLastAccessTimeUtc_WithLastAccessTimeUtcFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastAccessTimeUtc(lastAccessTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastAccessTimeUtc with lastAccessTimeUtc matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetLastAccessTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastAccessTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastAccessTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetLastWriteTimeTests
			{
				[Fact]
				public async Task SetLastWriteTime_WithLastWriteTimeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastWriteTime(lastWriteTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastWriteTime with lastWriteTime matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetLastWriteTime_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastWriteTime(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastWriteTime with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetLastWriteTimeUtcTests
			{
				[Fact]
				public async Task SetLastWriteTimeUtc_WithLastWriteTimeUtcFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastWriteTimeUtc(lastWriteTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastWriteTimeUtc with lastWriteTimeUtc matching t => t.Year == 2000 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetLastWriteTimeUtc_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.SetLastWriteTimeUtc(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.SetLastWriteTimeUtc with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task ResolveLinkTarget_WithLinkPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.ResolveLinkTarget(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.ResolveLinkTarget with linkPath matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ResolveLinkTarget_WithReturnFinalTargetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.ResolveLinkTarget(returnFinalTarget: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.ResolveLinkTarget with returnFinalTarget matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");
					_ = fileSystem.Directory.ResolveLinkTarget("link", false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithReturnFinalTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");
					_ = fileSystem.Directory.ResolveLinkTarget("link", true);
					_ = fileSystem.Directory.ResolveLinkTarget("link", false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.ResolveLinkTarget(returnFinalTarget: r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class CreateSymbolicLinkTests
			{
				[Fact]
				public async Task CreateSymbolicLink_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateSymbolicLink(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateSymbolicLink with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task CreateSymbolicLink_WithPathToTargetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateSymbolicLink(pathToTarget: t => t == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateSymbolicLink with pathToTarget matching t => t == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldMatchOnce()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("target");
					fileSystem.Directory.CreateSymbolicLink("link", "target");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateSymbolicLink().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathToTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					fileSystem.Directory.CreateSymbolicLink("link-a", "a");
					fileSystem.Directory.CreateSymbolicLink("link-b", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateSymbolicLink(pathToTarget: t => t == "a").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTempSubdirectoryTests
			{
				[Fact]
				public async Task CreateTempSubdirectory_WithPrefixFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateTempSubdirectory(p => p == "foo").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to Directory.CreateTempSubdirectory with prefix matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.CreateTempSubdirectory("pre-");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Directory.CreateTempSubdirectory().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPrefixFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Directory.CreateTempSubdirectory("alpha-");
					_ = fileSystem.Directory.CreateTempSubdirectory("beta-");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Directory.CreateTempSubdirectory(p => p == "alpha-").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif
		}
	}
}
