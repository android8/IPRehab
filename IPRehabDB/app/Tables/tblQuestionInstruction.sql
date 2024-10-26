CREATE TABLE [app].[tblQuestionInstruction] (
    [InstructionID]     INT           IDENTITY (1, 1) NOT NULL,
    [QuestionIDFK]      INT           NOT NULL,
    [StageCodeSetIDFK]  INT           NULL,
    [Order]             INT           NULL,
    [DisplayLocationFK] INT           NOT NULL,
    [Instruction]       VARCHAR (MAX) NOT NULL,
    [LastUpdate]        DATETIME      NOT NULL,
    CONSTRAINT [PK_tblInstruction] PRIMARY KEY CLUSTERED ([InstructionID] ASC),
    CONSTRAINT [FK_tblQuestionInstruction_tblCodeSet_DisplayLocation] FOREIGN KEY ([DisplayLocationFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblQuestionInstruction_tblCodeSet_StageCode] FOREIGN KEY ([StageCodeSetIDFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID]),
    CONSTRAINT [FK_tblQuestionInstruction_tblQuestion] FOREIGN KEY ([QuestionIDFK]) REFERENCES [app].[tblQuestion] ([QuestionID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblInstruction]
    ON [app].[tblQuestionInstruction]([QuestionIDFK] ASC, [Order] ASC);

