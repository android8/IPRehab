CREATE TABLE [app].[tblAnswerScore] (
    [AnswerID] INT NOT NULL,
    [Score]    INT CONSTRAINT [DEFAULT_tblAnswerScore_Score] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_tblAnswerScore] PRIMARY KEY CLUSTERED ([AnswerID] ASC),
    CONSTRAINT [FK_tblAnswerScore_tblAnswer] FOREIGN KEY ([AnswerID]) REFERENCES [app].[tblAnswer] ([AnswerID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'score only GG0130 and GG0170 series answers', @level0type = N'SCHEMA', @level0name = N'app', @level1type = N'TABLE', @level1name = N'tblAnswerScore';

