CREATE TABLE [dbo].[PlayersInTables] (
    [TableID]  INT NOT NULL,
    [PlayerID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([TableID] ASC),
    CONSTRAINT [FK_Table_Table] FOREIGN KEY ([TableID]) REFERENCES [dbo].[Table] ([ID]),
    CONSTRAINT [FK_Table_Player] FOREIGN KEY ([PlayerID]) REFERENCES [dbo].[Player] ([ID])
);

