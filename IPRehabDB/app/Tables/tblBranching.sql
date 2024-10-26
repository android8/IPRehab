CREATE TABLE [app].[tblBranching] (
    [BranchingID]    INT           IDENTITY (1, 1) NOT NULL,
    [BranchingName]  VARCHAR (255) NOT NULL,
    [FromQuestionID] INT           NOT NULL,
    [ToQuestionID]   INT           NOT NULL,
    [Condition]      VARCHAR (250) NULL,
    CONSTRAINT [PK_tblBranching] PRIMARY KEY CLUSTERED ([BranchingID] ASC),
    CONSTRAINT [FK_tblBranching_tblQuestion_From] FOREIGN KEY ([FromQuestionID]) REFERENCES [app].[tblQuestion] ([QuestionID]),
    CONSTRAINT [FK_tblBranching_tblQuestion_To] FOREIGN KEY ([ToQuestionID]) REFERENCES [app].[tblQuestion] ([QuestionID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblBranching_To_And_From]
    ON [app].[tblBranching]([FromQuestionID] ASC, [ToQuestionID] ASC, [BranchingName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblBranching_Name]
    ON [app].[tblBranching]([BranchingName] ASC);

