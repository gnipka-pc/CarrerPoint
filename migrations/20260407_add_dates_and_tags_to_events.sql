-- Migration: Add StartDate, EndDate and Tags to Events
-- Date: 2026-04-07

-- Add StartDate and EndDate columns to Events table
ALTER TABLE `Events`
ADD COLUMN `StartDate` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
ADD COLUMN `EndDate` datetime(6) NULL;

-- Create EventTags table
CREATE TABLE `EventTags` (
    `Id` char(36) NOT NULL,
    `EventId` char(36) NOT NULL,
    `Key` varchar(255) NOT NULL,
    `Value` varchar(1000) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_EventTags_EventId_Key` (`EventId`, `Key`),
    CONSTRAINT `FK_EventTags_Events_EventId` FOREIGN KEY (`EventId`) REFERENCES `Events` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Example tags for Olympiad events:
-- INSERT INTO EventTags (Id, EventId, `Key`, `Value`) VALUES (UUID(), '<event-id>', 'direction', 'Code');
-- INSERT INTO EventTags (Id, EventId, `Key`, `Value`) VALUES (UUID(), '<event-id>', 'difficulty', 'Medium');
