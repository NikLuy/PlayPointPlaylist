-- Migration: Add Voting System with Security
-- Run this if you have an existing database

-- Step 1: Add UpVotes and DownVotes columns to QueueItems
ALTER TABLE QueueItems ADD COLUMN UpVotes INTEGER NOT NULL DEFAULT 0;
ALTER TABLE QueueItems ADD COLUMN DownVotes INTEGER NOT NULL DEFAULT 0;

-- Step 2: Create VoteRecords table for audit trail
CREATE TABLE IF NOT EXISTS VoteRecords (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    QueueItemId INTEGER NOT NULL,
    VoterIdentifier TEXT NOT NULL,
    IsUpVote INTEGER NOT NULL,
    VotedAt TEXT NOT NULL,
    IpAddress TEXT,
    FOREIGN KEY (QueueItemId) REFERENCES QueueItems(Id) ON DELETE CASCADE
);

-- Step 3: Create unique index to prevent duplicate votes
CREATE UNIQUE INDEX IF NOT EXISTS IX_VoteRecords_QueueItemId_VoterIdentifier 
ON VoteRecords(QueueItemId, VoterIdentifier);

-- Step 4: Create index for performance
CREATE INDEX IF NOT EXISTS IX_VoteRecords_VoterIdentifier 
ON VoteRecords(VoterIdentifier);

-- If you're starting fresh, just delete the old playlist.db file
-- and let EF Core create a new one with the updated schema

-- RECOMMENDATION: Delete old database for clean start
-- rm playlist.db (Linux/Mac) or del playlist.db (Windows)
