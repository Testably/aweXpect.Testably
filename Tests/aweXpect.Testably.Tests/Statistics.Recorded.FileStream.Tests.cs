using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class FileStream
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task New_WithAccessFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(access: a => a == FileAccess.Read).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.New with access matching a => a == FileAccess.Read exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithBufferSizeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(bufferSize: b => b == 4096).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.New with bufferSize matching b => b == 4096 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithModeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(mode: m => m == FileMode.Open).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.New with mode matching m => m == FileMode.Open exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithModeFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None))
					{
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(mode: m => m == FileMode.Open).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task New_WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(p => p == "a.txt").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.New with path matching p => p == "a.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task New_WithShareFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.New(share: s => s == FileShare.None).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.New with share matching s => s == FileShare.None exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Wrap_WithFileStreamFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream.Wrap(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream.Wrap with fileStream matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class WriteByteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.WriteByte(99);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteByte(v => v == 99).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteByte_WithValueFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].WriteByte(n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].WriteByte with value matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class LengthTests
			{
				[Fact]
				public async Task Length_Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Length;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Length.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Length_Get_WhenNotAccessed_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Length.Get().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a get of FileStream["a.txt"].Length exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class CloseTests
			{
				[Fact]
				public async Task Close_WhenNotCalled_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Close().Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["a.txt"].Close exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None);
					s.Close();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Close().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CopyToTests
			{
				[Fact]
				public async Task CopyTo_WithBufferSizeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].CopyTo(bufferSize: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].CopyTo with bufferSize matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task CopyTo_WithDestinationFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].CopyTo(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].CopyTo with destination matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream src = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					using (MemoryStream dest = new())
					{
						src.CopyTo(dest, 4096);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CopyTo(bufferSize: b => b == 4096).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CopyToAsyncTests
			{
				[Fact]
				public async Task CopyToAsync_WithBufferSizeFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].CopyToAsync(bufferSize: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].CopyToAsync with bufferSize matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task CopyToAsync_WithDestinationFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].CopyToAsync(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].CopyToAsync with destination matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream src = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					using (MemoryStream dest = new())
					{
						await src.CopyToAsync(dest);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CopyToAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FlushTests
			{
				[Fact]
				public async Task Flush_WithFlushToDiskFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Flush(b => b).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Flush with flushToDisk matching b => b exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.Flush();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Flush().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FlushAsyncTests
			{
				[Fact]
				public async Task FlushAsync_WithCancellationTokenFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].FlushAsync(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].FlushAsync with cancellationToken matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						await s.FlushAsync();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].FlushAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadTests
			{
				[Fact]
				public async Task Read_WithBufferFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Read(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Read with buffer matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Read_WithCountFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Read(count: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Read with count matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Read_WithOffsetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Read(offset: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Read with offset matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						byte[] buffer = new byte[5];
						_ = s.Read(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Read(count: c => c == 5).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadAsyncTests
			{
				[Fact]
				public async Task ReadAsync_WithBufferFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].ReadAsync(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].ReadAsync with buffer matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAsync_WithCountFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].ReadAsync(count: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].ReadAsync with count matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task ReadAsync_WithOffsetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].ReadAsync(offset: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].ReadAsync with offset matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						byte[] buffer = new byte[5];
						_ = await s.ReadAsync(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadByteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.Read))
					{
						_ = s.ReadByte();
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadByte().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SeekTests
			{
				[Fact]
				public async Task Seek_WithOffsetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Seek(n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Seek with offset matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Seek_WithOriginFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Seek(origin: o => o == SeekOrigin.Begin).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Seek with origin matching o => o == SeekOrigin.Begin exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "hello");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open, FileAccess.ReadWrite))
					{
						s.Seek(1, SeekOrigin.Begin);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Seek(origin: o => o == SeekOrigin.Begin).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class SetLengthTests
			{
				[Fact]
				public async Task SetLength_WithValueFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].SetLength(n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].SetLength with value matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						s.SetLength(10);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].SetLength(v => v == 10).Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						byte[] buffer = new byte[]
						{
							1, 2, 3,
						};
						s.Write(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Write(count: c => c == 3).Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Write_WithBufferFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Write(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Write with buffer matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Write_WithCountFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Write(count: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Write with count matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task Write_WithOffsetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].Write(offset: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].Write with offset matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class WriteAsyncTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Create, FileAccess.Write, FileShare.None))
					{
						byte[] buffer = new byte[]
						{
							1, 2, 3,
						};
						await s.WriteAsync(buffer, 0, buffer.Length);
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteAsync().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WriteAsync_WithBufferFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].WriteAsync(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].WriteAsync with buffer matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAsync_WithCountFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].WriteAsync(count: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].WriteAsync with count matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WriteAsync_WithOffsetFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["foo.txt"].WriteAsync(offset: n => n == 0).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             recorded a call to FileStream["foo.txt"].WriteAsync with offset matching n => n == 0 exactly once,
						             but it was recorded 0 times
						             """);
				}
			}

			public sealed class CanReadTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanRead;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanRead.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanSeekTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanSeek;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanSeek.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanTimeout;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class CanWriteTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.CanWrite;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].CanWrite.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class IsAsyncTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.IsAsync;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].IsAsync.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NameTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Name;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Name.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class PositionTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						_ = s.Position;
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].Position.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ReadTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						try { _ = s.ReadTimeout; }
						catch (InvalidOperationException) { }
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].ReadTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WriteTimeoutTests
			{
				[Fact]
				public async Task Get_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "x");
					using (FileSystemStream s = fileSystem.FileStream.New("a.txt", FileMode.Open))
					{
						try { _ = s.WriteTimeout; }
						catch (InvalidOperationException) { }
					}

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileStream["a.txt"].WriteTimeout.Get().Once();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
