CREATE TABLE [dbo].[Table] (
    [ID]                INT        NOT NULL,
    [Max Players]       NCHAR (10) NOT NULL,
    [Number Of Players] NCHAR (10) NOT NULL,
    [Small Blind]       INT        NOT NULL,
    [Big Blind]         INT        NOT NULL,
    [Name] NCHAR(10) NOT NULL, 
    [AmountToEnter] INT NOT NULL DEFAULT 1, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

