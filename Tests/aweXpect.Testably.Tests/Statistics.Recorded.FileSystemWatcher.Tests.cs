using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileSystemWatcher
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task New_WithFilterFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher.New(filter: f => f == "*.txt").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher.New with filter matching f => f == "*.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher.New(p => p == "a").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher.New with path matching p => p == "a" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("a");
					fileSystem.Directory.CreateDirectory("b");
					_ = fileSystem.FileSystemWatcher.New("a");
					_ = fileSystem.FileSystemWatcher.New("b");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher.New(p => p == "a").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Wrap_WithFileSystemWatcherFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher.Wrap(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher.Wrap with fileSystemWatcher matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class BeginInitTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.BeginInit();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].BeginInit().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FilterPropertyTests
			{
				[Fact]
				public async Task Filter_Set_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.Filter = "*.txt";

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Filter.Set().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EndInitTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.BeginInit();
					w.EndInit();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].EndInit().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WaitForChangedTests
			{
				[Fact]
				public async Task WaitForChanged_WithChangeTypeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"]
							.WaitForChanged(c => c == WatcherChangeTypes.All).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher["watch"].WaitForChanged with changeType matching c => c == WatcherChangeTypes.All exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WaitForChanged_WithTimeoutFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"]
							.WaitForChanged(timeout: t => t == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher["watch"].WaitForChanged with timeout matching t => t == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledWithTimeout_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.WaitForChanged(WatcherChangeTypes.All, 0);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"]
							.WaitForChanged(timeout: t => t == 0).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnableRaisingEventsTests
			{
				[Fact]
				public async Task Set_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					w.EnableRaisingEvents = false;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].EnableRaisingEvents.Set().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class FiltersTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.Filters;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Filters.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class IncludeSubdirectoriesTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.IncludeSubdirectories;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].IncludeSubdirectories.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class InternalBufferSizeTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.InternalBufferSize;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].InternalBufferSize.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NotifyFilterTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.NotifyFilter;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].NotifyFilter.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PathTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.Path;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Path.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SiteTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.Site;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Site.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SynchronizingObjectTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");
					IFileSystemWatcher w = fileSystem.FileSystemWatcher.New("watch");
					_ = w.SynchronizingObject;

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].SynchronizingObject.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FailureMessageTests
			{
				[Fact]
				public async Task EnableRaisingEvents_Set_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].EnableRaisingEvents.Set().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a set of FileSystemWatcher["watch"].EnableRaisingEvents exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task EndInit_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].EndInit().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher["watch"].EndInit exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Path_Get_WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"].Path.Get().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a get of FileSystemWatcher["watch"].Path exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WaitForChanged_WithFilter_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();
					fileSystem.Directory.CreateDirectory("watch");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileSystemWatcher["watch"]
							.WaitForChanged(timeout: t => t == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileSystemWatcher["watch"].WaitForChanged with timeout matching t => t == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
