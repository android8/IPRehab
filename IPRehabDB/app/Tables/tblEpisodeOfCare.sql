CREATE TABLE [app].[tblEpisodeOfCare] (
    [EpisodeOfCareID] INT          IDENTITY (1, 1) NOT NULL,
    [OnsetDate]       DATE         NOT NULL,
    [AdmissionDate]   DATE         NOT NULL,
    [PatientICNFK]    VARCHAR (10) NOT NULL,
    [FacilityID6]     NCHAR (6)    NULL,
    [LastUpdate]      DATETIME     NOT NULL,
    [Completed]       BIT          NULL,
    CONSTRAINT [PK_app.tblEpisodeOfCare] PRIMARY KEY CLUSTERED ([EpisodeOfCareID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ICN_AdmitDate]
    ON [app].[tblEpisodeOfCare]([PatientICNFK] ASC, [AdmissionDate] ASC);

