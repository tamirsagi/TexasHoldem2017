CREATE TABLE [dbo].[Player] (
    [ID]          INT        IDENTITY (1, 1) NOT NULL,
    [FirstName]   NCHAR (10) NOT NULL,
    [LastName]    NCHAR (10) NOT NULL,
    [UserName]    NCHAR (10) NOT NULL,
    [pwd]         NCHAR (10) NOT NULL,
    [Total Money] INT        DEFAULT ((1000)) NOT NULL,
    [BestHand] NCHAR(5) NOT NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

