CREATE TABLE [app].[tblUser] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [FirstName]   VARCHAR (20) NOT NULL,
    [LastName]    VARCHAR (20) NOT NULL,
    [NetworkName] NCHAR (20)   NOT NULL,
    [LastUpdate]  DATETIME     NOT NULL,
    CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblUser_Name]
    ON [app].[tblUser]([LastName] ASC, [FirstName] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblUser_NetworkName]
    ON [app].[tblUser]([NetworkName] ASC);

