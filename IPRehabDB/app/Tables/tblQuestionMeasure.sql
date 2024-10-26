CREATE TABLE [app].[tblQuestionMeasure] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [QuestionIDFK]       INT           NOT NULL,
    [StageFK]            INT           NOT NULL,
    [Required]           BIT           CONSTRAINT [DF__tblQuesti__Requi__7FB5F315] DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [MeasureCodeSetIDFK] INT           NULL,
    [Comment]            VARCHAR (500) NULL,
    [LastUpdate]         DATETIME      NOT NULL,
    CONSTRAINT [PK_tblQuestionMeasure] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_tblQuestionMeasure_tblCodeSet] FOREIGN KEY ([StageFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblQuestionMeasure_tblCodeSet1] FOREIGN KEY ([MeasureCodeSetIDFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblQuestionMeasure_tblQuestion] FOREIGN KEY ([QuestionIDFK]) REFERENCES [app].[tblQuestion] ([QuestionID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblQuestionMeasure]
    ON [app].[tblQuestionMeasure]([QuestionIDFK] ASC, [StageFK] ASC, [MeasureCodeSetIDFK] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblQuestionMeasure_QuestionIdFk]
    ON [app].[tblQuestionMeasure]([QuestionIDFK] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblQuestionMeasure_StageFk]
    ON [app].[tblQuestionMeasure]([StageFK] ASC);

