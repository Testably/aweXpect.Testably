using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Path
		{
			public sealed class AltDirectorySeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.AltDirectorySeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.AltDirectorySeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CombineTests
			{
				[Fact]
				public async Task Combine_WithPath1Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Combine with path1 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Combine_WithPath2Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path2: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Combine with path2 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Combine_WithPath3Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path3: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Combine with path3 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Combine_WithPath4Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path4: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Combine with path4 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task FilterOnPath3_ShouldOnlyMatchThreeArgOverload()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Combine("a", "b");
					_ = fileSystem.Path.Combine("a", "b", "c");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(path3: p => p == "c").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCombineWithTwoArgs_ShouldMatch()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Combine("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Combine(p => p == "a", p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DirectorySeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.DirectorySeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.DirectorySeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFileNameTests
			{
				[Fact]
				public async Task GetFileName_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFileName(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetFileName with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetFileName("dir/foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFileName(p => p == "dir/foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotCalled_ShouldFailWithOnce()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.GetFileName().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetFileName exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class GetRelativePathTests
			{
				[Fact]
				public async Task GetRelativePath_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetRelativePath(path: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetRelativePath with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetRelativePath_WithRelativeToFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetRelativePath(r => r == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetRelativePath with relativeTo matching r => r == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetRelativePath("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetRelativePath(r => r == "a", p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class GetTempPathTests
			{
				[Fact]
				public async Task NoArgMethod_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetTempPath();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.GetTempPath().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PathSeparatorTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.PathSeparator;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.PathSeparator.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class TrimEndingDirectorySeparatorTests
			{
				[Fact]
				public async Task TrimEndingDirectorySeparator_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.TrimEndingDirectorySeparator(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.TrimEndingDirectorySeparator with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.TrimEndingDirectorySeparator("dir/");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.TrimEndingDirectorySeparator(p => p == "dir/").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class VolumeSeparatorCharTests
			{
				[Fact]
				public async Task WhenRead_ShouldRecordGet()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.VolumeSeparatorChar;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.VolumeSeparatorChar.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ChangeExtensionTests
			{
				[Fact]
				public async Task ChangeExtension_WithExtensionFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.ChangeExtension(extension: e => e == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.ChangeExtension with extension matching e => e == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ChangeExtension_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.ChangeExtension(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.ChangeExtension with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.ChangeExtension("foo.txt", ".bin");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.ChangeExtension(p => p == "foo.txt", e => e == ".bin").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetDirectoryNameTests
			{
				[Fact]
				public async Task GetDirectoryName_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetDirectoryName(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetDirectoryName with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetDirectoryName("dir/foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetDirectoryName(p => p == "dir/foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetExtensionTests
			{
				[Fact]
				public async Task GetExtension_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetExtension(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetExtension with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetExtension("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetExtension(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFileNameWithoutExtensionTests
			{
				[Fact]
				public async Task GetFileNameWithoutExtension_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFileNameWithoutExtension(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetFileNameWithoutExtension with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetFileNameWithoutExtension("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFileNameWithoutExtension(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetFullPathTests
			{
				[Fact]
				public async Task GetFullPath_WithBasePathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFullPath(basePath: b => b == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetFullPath with basePath matching b => b == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task GetFullPath_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFullPath(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetFullPath with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetFullPath("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetFullPath(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetInvalidFileNameCharsTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetInvalidFileNameChars();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetInvalidFileNameChars().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetInvalidPathCharsTests
			{
				[Fact]
				public async Task GetInvalidPathChars_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.GetInvalidPathChars().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetInvalidPathChars exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetInvalidPathChars();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetInvalidPathChars().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetPathRootTests
			{
				[Fact]
				public async Task GetPathRoot_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetPathRoot(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.GetPathRoot with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetPathRoot("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetPathRoot().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetRandomFileNameTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.GetRandomFileName();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetRandomFileName().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetTempFileNameTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
#pragma warning disable CS0618
					_ = fileSystem.Path.GetTempFileName();
#pragma warning restore CS0618

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.GetTempFileName().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class HasExtensionTests
			{
				[Fact]
				public async Task HasExtension_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.HasExtension(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.HasExtension with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.HasExtension("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.HasExtension(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class IsPathRootedTests
			{
				[Fact]
				public async Task IsPathRooted_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.IsPathRooted(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.IsPathRooted with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.IsPathRooted("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.IsPathRooted(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class EndsInDirectorySeparatorTests
			{
				[Fact]
				public async Task EndsInDirectorySeparator_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.EndsInDirectorySeparator(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.EndsInDirectorySeparator with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.EndsInDirectorySeparator("dir/");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.EndsInDirectorySeparator(p => p == "dir/").Once();
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
							.Path.Exists(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Exists with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Exists("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.Exists().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class IsPathFullyQualifiedTests
			{
				[Fact]
				public async Task IsPathFullyQualified_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.IsPathFullyQualified(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.IsPathFullyQualified with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.IsPathFullyQualified("/abs");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().Path.IsPathFullyQualified().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class JoinTests
			{
				[Fact]
				public async Task FilterOnPath4_ShouldOnlyMatchFourArgOverload()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Join("a", "b");
					_ = fileSystem.Path.Join("a", "b", "c", "d");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path4: p => p == "d").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Join_WithPath1Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Join with path1 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Join_WithPath2Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path2: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Join with path2 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Join_WithPath3Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path3: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Join with path3 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Join_WithPath4Filter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(path4: p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to Path.Join with path4 matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenJoinedTwoArgs_ShouldMatch()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.Path.Join("a", "b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.Path.Join(p => p == "a", p => p == "b").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif
		}
	}
}
