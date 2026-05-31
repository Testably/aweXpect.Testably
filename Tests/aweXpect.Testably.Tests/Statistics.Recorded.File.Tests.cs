using System.IO;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class File
		{
			public sealed class AppendAllBytesTests
			{
				[Fact]
				public async Task AppendAllBytes_WithBytesFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllBytes(bytes: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllBytes with bytes matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllBytes_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllBytes(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllBytes with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenNeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllBytes().Never();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendAllBytesAsyncTests
			{
				[Fact]
				public async Task AppendAllBytesAsync_WithBytesFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllBytesAsync(bytes: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllBytesAsync with bytes matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllBytesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllBytesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllBytesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenNeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllBytesAsync().Never();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CopyTests
			{
				[Fact]
				public async Task Copy_WithDestFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Copy(destFileName: p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Copy with destFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Copy_WithOverwriteFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Copy(overwrite: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Copy with overwrite matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Copy_WithSourceFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Copy(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Copy with sourceFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCopiedTwice_ShouldSucceedWithTwice()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.Copy("a.txt", "b.txt");
					fileSystem.File.Copy("a.txt", "c.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Copy().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithDestFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.Copy("a.txt", "b.txt");
					fileSystem.File.Copy("a.txt", "c.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Copy(destFileName: d => d == "b.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

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
							.File.CreateSymbolicLink(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.CreateSymbolicLink with path matching p => p == "foo.txt" exactly once,
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
							.File.CreateSymbolicLink(pathToTarget: p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.CreateSymbolicLink with pathToTarget matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.CreateSymbolicLink().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathToTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.WriteAllText("b.txt", "y");
					fileSystem.File.CreateSymbolicLink("link-a.txt", "a.txt");
					fileSystem.File.CreateSymbolicLink("link-b.txt", "b.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.CreateSymbolicLink(pathToTarget: t => t == "a.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class DeleteTests
			{
				[Fact]
				public async Task CalledOnce_ShouldFailExpectingNever()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					fileSystem.File.Delete("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Delete().Never();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded no call to File.Delete,
						             but it was recorded 1 time
						             """);
				}

				[Fact]
				public async Task Delete_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Delete(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Delete with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task NeverCalled_ShouldSucceedWithNever()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Delete().Never();
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
							.File.Exists(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Exists with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenExistsCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.File.Exists("foo.txt");
					_ = fileSystem.File.Exists("bar.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.Exists().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.File.Exists("foo.txt");
					_ = fileSystem.File.Exists("bar.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Exists(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteAllTextTests
			{
				[Fact]
				public async Task FilteringOnEncoding_ShouldOnlyMatchThreeArgOverload()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");
					fileSystem.File.WriteAllText("bar.txt", "world", Encoding.UTF8);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(encoding: _ => true).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenCalledTwice_ShouldSucceedExpectingTwice()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.WriteAllText("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNotCalled_ShouldFailExpectingOnce()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllText exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WithMatchingPathPredicate_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "hello");
					fileSystem.File.WriteAllText("bar.txt", "world");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteAllText_WithContentsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(contents: c => c == "hello").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllText with contents matching c => c == "hello" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllText_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllText with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class WriteAllTextAsyncTests
			{
				[Fact]
				public async Task WhenCalledTwice_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllTextAsync("a.txt", "x");
					await fileSystem.File.WriteAllTextAsync("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllTextAsync().Twice();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithMatchingPathPredicate_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllTextAsync("foo.txt", "hello");
					await fileSystem.File.WriteAllTextAsync("bar.txt", "world");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllTextAsync(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteAllTextAsync_WithContentsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllTextAsync(contents: c => c == "hello").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllTextAsync with contents matching c => c == "hello" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllTextAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllTextAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllTextAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllTextAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllTextAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllTextAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
#endif

			public sealed class AppendAllLinesTests
			{
				[Fact]
				public async Task AppendAllLines_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLines(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllLines with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllLines_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLines(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllLines with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.AppendAllLines("foo.txt", new[]
					{
						"line",
					});

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLines(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendAllTextTests
			{
				[Fact]
				public async Task AppendAllText_WithContentsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllText(contents: c => c == "hello").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllText with contents matching c => c == "hello" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllText_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllText(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllText with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.AppendAllText("foo.txt", "hello");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllText(contents: c => c == "hello").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendTextTests
			{
				[Fact]
				public async Task AppendText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.AppendText("foo.txt").Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendText(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTests
			{
				[Fact]
				public async Task Create_WithBufferSizeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Create(bufferSize: n => n == 0).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Create with bufferSize matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Create_WithOptionsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Create(options: o => o == FileOptions.None).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Create with options matching o => o == FileOptions.None exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Create_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Create(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Create with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.Create("foo.txt").Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Create(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CreateTextTests
			{
				[Fact]
				public async Task CreateText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.CreateText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.CreateText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.CreateText("foo.txt").Dispose();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.CreateText(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecryptTests
			{
				[Fact]
				public async Task Decrypt_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Decrypt(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Decrypt with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					try
					{
#pragma warning disable CA1416
						fileSystem.File.Decrypt("foo.txt");
#pragma warning restore CA1416
					}
					catch (PlatformNotSupportedException)
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Decrypt(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EncryptTests
			{
				[Fact]
				public async Task Encrypt_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Encrypt(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Encrypt with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					try
					{
#pragma warning disable CA1416
						fileSystem.File.Encrypt("foo.txt");
#pragma warning restore CA1416
					}
					catch (PlatformNotSupportedException)
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Encrypt(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GetAttributesTests
			{
				[Fact]
				public async Task GetAttributes_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetAttributes(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetAttributes with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetAttributes("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetAttributes().Once();
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
							.File.GetCreationTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetCreationTime with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetCreationTime("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetCreationTime().Once();
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
							.File.GetCreationTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetCreationTimeUtc with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetCreationTimeUtc("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetCreationTimeUtc().Once();
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
							.File.GetLastAccessTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetLastAccessTime with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetLastAccessTime("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetLastAccessTime().Once();
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
							.File.GetLastAccessTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetLastAccessTimeUtc with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetLastAccessTimeUtc("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetLastAccessTimeUtc().Once();
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
							.File.GetLastWriteTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetLastWriteTime with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetLastWriteTime("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetLastWriteTime().Once();
					}

					await That(Act).DoesNotThrow();
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
							.File.GetLastWriteTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetLastWriteTimeUtc with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "");
					_ = fileSystem.File.GetLastWriteTimeUtc("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetLastWriteTimeUtc().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class MoveTests
			{
				[Fact]
				public async Task Move_WithDestFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Move(destFileName: p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Move with destFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Move_WithOverwriteFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Move(overwrite: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Move with overwrite matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Move_WithSourceFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Move(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Move with sourceFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.Move("a.txt", "b.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Move(s => s == "a.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OpenTests
			{
				[Fact]
				public async Task Open_WithAccessFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Open(access: a => a == FileAccess.Read).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Open with access matching a => a == FileAccess.Read exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Open_WithModeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Open(mode: m => m == FileMode.Open).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Open with mode matching m => m == FileMode.Open exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Open_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Open(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Open with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Open_WithShareFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Open(share: s => s == FileShare.None).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Open with share matching s => s == FileShare.None exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class OpenReadTests
			{
				[Fact]
				public async Task OpenRead_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.OpenRead(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.OpenRead with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class OpenTextTests
			{
				[Fact]
				public async Task OpenText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.OpenText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.OpenText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class OpenWriteTests
			{
				[Fact]
				public async Task OpenWrite_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.OpenWrite(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.OpenWrite with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class ReadAllBytesTests
			{
				[Fact]
				public async Task ReadAllBytes_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllBytes(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllBytes with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class ReadAllLinesTests
			{
				[Fact]
				public async Task ReadAllLines_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllLines(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllLines with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAllLines_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllLines(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllLines with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class ReadAllTextTests
			{
				[Fact]
				public async Task ReadAllText_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllText(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllText with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAllText_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllText(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllText with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class ReadLinesTests
			{
				[Fact]
				public async Task ReadLines_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadLines(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadLines with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadLines_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadLines(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadLines with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class ReplaceTests
			{
				[Fact]
				public async Task Replace_WithDestinationBackupFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Replace(destinationBackupFileName: p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Replace with destinationBackupFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Replace_WithDestinationFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Replace(destinationFileName: p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Replace with destinationFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Replace_WithIgnoreMetadataErrorsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Replace(ignoreMetadataErrors: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Replace with ignoreMetadataErrors matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Replace_WithSourceFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.Replace(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.Replace with sourceFileName matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class SetAttributesTests
			{
				[Fact]
				public async Task SetAttributes_WithFileAttributesFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetAttributes(fileAttributes: a => a == FileAttributes.ReadOnly).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetAttributes with fileAttributes matching a => a == FileAttributes.ReadOnly exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetAttributes_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetAttributes(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetAttributes with path matching p => p == "foo.txt" exactly once,
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
							.File.SetCreationTime(creationTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetCreationTime with creationTime matching t => t.Year == 2000 exactly once,
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
							.File.SetCreationTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetCreationTime with path matching p => p == "foo.txt" exactly once,
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
							.File.SetCreationTimeUtc(creationTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetCreationTimeUtc with creationTimeUtc matching t => t.Year == 2000 exactly once,
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
							.File.SetCreationTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetCreationTimeUtc with path matching p => p == "foo.txt" exactly once,
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
							.File.SetLastAccessTime(lastAccessTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastAccessTime with lastAccessTime matching t => t.Year == 2000 exactly once,
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
							.File.SetLastAccessTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastAccessTime with path matching p => p == "foo.txt" exactly once,
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
							.File.SetLastAccessTimeUtc(lastAccessTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastAccessTimeUtc with lastAccessTimeUtc matching t => t.Year == 2000 exactly once,
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
							.File.SetLastAccessTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastAccessTimeUtc with path matching p => p == "foo.txt" exactly once,
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
							.File.SetLastWriteTime(lastWriteTime: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastWriteTime with lastWriteTime matching t => t.Year == 2000 exactly once,
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
							.File.SetLastWriteTime(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastWriteTime with path matching p => p == "foo.txt" exactly once,
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
							.File.SetLastWriteTimeUtc(lastWriteTimeUtc: t => t.Year == 2000).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastWriteTimeUtc with lastWriteTimeUtc matching t => t.Year == 2000 exactly once,
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
							.File.SetLastWriteTimeUtc(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetLastWriteTimeUtc with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class WriteAllBytesTests
			{
				[Fact]
				public async Task WriteAllBytes_WithBytesFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllBytes(bytes: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllBytes with bytes matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllBytes_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllBytes(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllBytes with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class WriteAllLinesTests
			{
				[Fact]
				public async Task WriteAllLines_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllLines(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllLines with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllLines_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllLines(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllLines with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class AppendAllLinesAsyncTests
			{
				[Fact]
				public async Task AppendAllLinesAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLinesAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllLinesAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllLinesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllLinesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllLinesAsync("foo.txt", new[]
					{
						"line",
					});

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllLinesAsync("foo.txt", new[]
					{
						"line",
					});
					await fileSystem.File.AppendAllLinesAsync("bar.txt", new[]
					{
						"line",
					});

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AppendAllTextAsyncTests
			{
				[Fact]
				public async Task AppendAllTextAsync_WithContentsFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllTextAsync(contents: c => c == "hello").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllTextAsync with contents matching c => c == "hello" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllTextAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllTextAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllTextAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task AppendAllTextAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.AppendAllTextAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.AppendAllTextAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledTwice_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.AppendAllTextAsync("a.txt", "x");
					await fileSystem.File.AppendAllTextAsync("b.txt", "y");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.AppendAllTextAsync().Twice();
					}

					await That(Act).DoesNotThrow();
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class GetUnixFileModeTests
			{
				[Fact]
				public async Task GetUnixFileMode_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.GetUnixFileMode(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.GetUnixFileMode with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					_ = fileSystem.File.GetUnixFileMode("foo.txt");
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.GetUnixFileMode().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllBytesAsyncTests
			{
				[Fact]
				public async Task ReadAllBytesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllBytesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllBytesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllBytesAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllBytesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllLinesAsyncTests
			{
				[Fact]
				public async Task ReadAllLinesAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllLinesAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllLinesAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAllLinesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllLinesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllLinesAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAllTextAsyncTests
			{
				[Fact]
				public async Task ReadAllTextAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllTextAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllTextAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAllTextAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadAllTextAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadAllTextAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await fileSystem.File.ReadAllTextAsync("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadAllTextAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadLinesAsyncTests
			{
				[Fact]
				public async Task ReadLinesAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadLinesAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadLinesAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadLinesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ReadLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ReadLinesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("foo.txt", "x");
					await foreach (string _ in fileSystem.File.ReadLinesAsync("foo.txt"))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ReadLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ResolveLinkTargetTests
			{
				[Fact]
				public async Task ResolveLinkTarget_WithLinkPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ResolveLinkTarget(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ResolveLinkTarget with linkPath matching p => p == "foo.txt" exactly once,
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
							.File.ResolveLinkTarget(returnFinalTarget: b => b).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.ResolveLinkTarget with returnFinalTarget matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");
					_ = fileSystem.File.ResolveLinkTarget("link.txt", false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.ResolveLinkTarget().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithReturnFinalTargetFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("target.txt", "hello");
					fileSystem.File.CreateSymbolicLink("link.txt", "target.txt");
					_ = fileSystem.File.ResolveLinkTarget("link.txt", true);
					_ = fileSystem.File.ResolveLinkTarget("link.txt", false);

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.ResolveLinkTarget(returnFinalTarget: r => r).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SetUnixFileModeTests
			{
				[Fact]
				public async Task SetUnixFileMode_WithModeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetUnixFileMode(mode: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetUnixFileMode with mode matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task SetUnixFileMode_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetUnixFileMode(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.SetUnixFileMode with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					fileSystem.File.SetUnixFileMode("foo.txt", UnixFileMode.UserRead);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.SetUnixFileMode().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithModeFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Linux));
					fileSystem.File.WriteAllText("foo.txt", "");
#pragma warning disable CA1416
					fileSystem.File.SetUnixFileMode("foo.txt", UnixFileMode.UserRead);
					fileSystem.File.SetUnixFileMode("foo.txt", UnixFileMode.UserWrite);
#pragma warning restore CA1416

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.SetUnixFileMode(mode: m => m == UnixFileMode.UserRead).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteAllBytesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllBytesAsync("foo.txt", new byte[]
					{
						1, 2, 3,
					});

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllBytesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteAllBytesAsync_WithBytesFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllBytesAsync(bytes: _ => true).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllBytesAsync with bytes matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllBytesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllBytesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllBytesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class WriteAllLinesAsyncTests
			{
				[Fact]
				public async Task WhenCalledOnce_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					await fileSystem.File.WriteAllLinesAsync("foo.txt", new[]
					{
						"line",
					});

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().File.WriteAllLinesAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteAllLinesAsync_WithEncodingFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllLinesAsync(encoding: e => e == Encoding.UTF8).Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllLinesAsync with encoding matching e => e == Encoding.UTF8 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAllLinesAsync_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.File.WriteAllLinesAsync(p => p == "foo.txt").Once();
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to File.WriteAllLinesAsync with path matching p => p == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
#endif
		}
	}
}
