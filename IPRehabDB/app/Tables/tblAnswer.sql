CREATE TABLE [app].[tblAnswer] (
    [AnswerID]             INT           IDENTITY (1, 1) NOT NULL,
    [EpsideOfCareIDFK]     INT           NOT NULL,
    [QuestionIDFK]         INT           NOT NULL,
    [MeasureIDFK]          INT           NOT NULL,
    [AnswerCodeSetFK]      INT           NOT NULL,
    [AnswerSequenceNumber] INT           NOT NULL,
    [Description]          VARCHAR (MAX) NULL,
    [AnswerByUserID]       VARCHAR (50)  NOT NULL,
    [LastUpdate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_tblAnswer] PRIMARY KEY CLUSTERED ([AnswerID] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_tblAnswer_tblCodeSet_AnswerCodeSet] FOREIGN KEY ([AnswerCodeSetFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblAnswer_tblEpisodeOfCare] FOREIGN KEY ([EpsideOfCareIDFK]) REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID]),
    CONSTRAINT [FK_tblAnswer_tblQuestion] FOREIGN KEY ([QuestionIDFK]) REFERENCES [app].[tblQuestion] ([QuestionID]),
    CONSTRAINT [FK_tblAnswer_tblQuestionMeasure] FOREIGN KEY ([MeasureIDFK]) REFERENCES [app].[tblQuestionMeasure] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblAnswer_EpisodeOfCareIDFK]
    ON [app].[tblAnswer]([EpsideOfCareIDFK] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_tblAnswer_AnswerCodeSetFK]
    ON [app].[tblAnswer]([AnswerCodeSetFK] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_tblAnswer_QuestionIDFK]
    ON [app].[tblAnswer]([QuestionIDFK] ASC) WITH (FILLFACTOR = 90);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblAnswer]
    ON [app].[tblAnswer]([EpsideOfCareIDFK] ASC, [MeasureIDFK] ASC, [AnswerCodeSetFK] ASC, [AnswerSequenceNumber] ASC) WITH (FILLFACTOR = 90);


GO
CREATE COLUMNSTORE INDEX [IX_tblAnswer_ColumnStore_QuestionID]
    ON [app].[tblAnswer]([QuestionIDFK]);

