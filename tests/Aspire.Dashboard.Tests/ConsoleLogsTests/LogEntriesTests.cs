// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Dashboard.ConsoleLogs;
using Aspire.Dashboard.Model;
using Xunit;

namespace Aspire.Dashboard.Tests.ConsoleLogsTests;

public class LogEntriesTests
{
    [Fact]
    public void InsertSorted_OutOfOrderWithSameTimestamp_ReturnInOrder()
    {
        // Arrange
        var logEntries = new LogEntries(maximumEntryCount: int.MaxValue);

        var timestamp = new DateTimeOffset(2024, 6, 25, 15, 59, 0, TimeSpan.Zero);

        logEntries.BaseLineNumber = 1;

        // Act
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp, Content = "1" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(1), Content = "3" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp, Content = "2" });

        // Assert
        var entries = logEntries.GetEntries();
        Assert.Collection(entries,
            l => Assert.Equal("1", l.Content),
            l => Assert.Equal("2", l.Content),
            l => Assert.Equal("3", l.Content));
    }

    [Fact]
    public void InsertSorted_TrimsToMaximumEntryCount_Ordered()
    {
        // Arrange
        var logEntries = new LogEntries(maximumEntryCount: 2) { BaseLineNumber = 1 };

        var timestamp = DateTimeOffset.UtcNow;

        // Act
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(1), Content = "1" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(2), Content = "2" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(3), Content = "3" });

        // Assert
        var entries = logEntries.GetEntries();
        Assert.Collection(entries,
            l => Assert.Equal("2", l.Content),
            l => Assert.Equal("3", l.Content));
    }

    [Fact]
    public void InsertSorted_TrimsToMaximumEntryCount_OutOfOrder()
    {
        // Arrange
        var logEntries = new LogEntries(maximumEntryCount: 2) { BaseLineNumber = 1 };

        var timestamp = DateTimeOffset.UtcNow;

        // Act
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(1), Content = "1" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(3), Content = "3" });
        logEntries.InsertSorted(new LogEntry { Timestamp = timestamp.AddSeconds(2), Content = "2" });

        // Assert
        var entries = logEntries.GetEntries();
        Assert.Collection(entries,
            l => Assert.Equal("2", l.Content),
            l => Assert.Equal("3", l.Content));
    }
}
