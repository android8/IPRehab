CREATE TABLE [app].[tblSignature] (
    [EpisodeCareIDFK]         INT           NOT NULL,
    [Signature]               BINARY (50)   NOT NULL,
    [Title]                   VARCHAR (100) NOT NULL,
    [DateInformationProvided] DATE          NOT NULL,
    [Time]                    TIME (7)      NULL,
    [LastUpdate]              DATETIME      NOT NULL,
    CONSTRAINT [PK_tblSignature] PRIMARY KEY CLUSTERED ([EpisodeCareIDFK] ASC),
    CONSTRAINT [FK_tblSignature_tblEpisodeOfCare] FOREIGN KEY ([EpisodeCareIDFK]) REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSignature]
    ON [app].[tblSignature]([Signature] ASC);

