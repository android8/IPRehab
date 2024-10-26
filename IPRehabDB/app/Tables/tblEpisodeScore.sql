CREATE TABLE [app].[tblEpisodeScore] (
    [EpisodeOfCareId]              INT NOT NULL,
    [Stage]                        INT CONSTRAINT [DEFAULT_tblScore_Stage] DEFAULT ((0)) NOT NULL,
    [SelfCareScore_Adm_Perf]       INT CONSTRAINT [DEFAULT_tblScore_SelfCareScore_Adm_Perf] DEFAULT ((0)) NOT NULL,
    [SelfCareScore_Discharge_Perf] INT CONSTRAINT [DEFAULT_tblScore_SelfCareScore_Discharge_Perf] DEFAULT ((0)) NOT NULL,
    [MobilityScore_Adm_Perf]       INT CONSTRAINT [DEFAULT_tblScore_MobilityScore_Adm_Perf] DEFAULT ((0)) NOT NULL,
    [MobilityScore_Discharge_Perf] INT CONSTRAINT [DEFAULT_tblScore_MobilityScore_Discharge_Perf] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_tblEpisodeScore] PRIMARY KEY CLUSTERED ([EpisodeOfCareId] ASC),
    CONSTRAINT [FK_tblEpisodeScore_tblCodeSet] FOREIGN KEY ([Stage]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblEpisodeScore_tblEpisodeOfCare] FOREIGN KEY ([EpisodeOfCareId]) REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
);

