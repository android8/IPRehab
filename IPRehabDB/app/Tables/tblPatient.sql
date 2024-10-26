CREATE TABLE [app].[tblPatient] (
    [ICN]         VARCHAR (10) NOT NULL,
    [IEN]         VARCHAR (10) NOT NULL,
    [FirstName]   VARCHAR (20) NOT NULL,
    [LastName]    VARCHAR (20) NOT NULL,
    [MiddleName]  VARCHAR (20) NULL,
    [DateOfBirth] DATE         NOT NULL,
    [Last4SSN]    CHAR (4)     NOT NULL,
    [LastUpdate]  DATETIME     NOT NULL,
    CONSTRAINT [PK_tblPatient] PRIMARY KEY CLUSTERED ([ICN] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblPatient_UniqueName]
    ON [app].[tblPatient]([LastName] ASC, [FirstName] ASC, [MiddleName] ASC, [Last4SSN] ASC);

