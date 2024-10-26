CREATE TABLE [app].[tblQuestion] (
    [QuestionID]      INT            IDENTITY (1, 1) NOT NULL,
    [QuestionKey]     VARCHAR (20)   NOT NULL,
    [Order]           INT            NULL,
    [QuestionSection] VARCHAR (200)  NULL,
    [Question]        VARCHAR (1000) NOT NULL,
    [GroupTitle]      VARCHAR (50)   NULL,
    [AnswerCodeSetFK] INT            NOT NULL,
    [BranchingPoint]  BIT            NULL,
    [MultiChoice]     BIT            NULL,
    [Active]          BIT            NULL,
    [LastUpdate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_app.tblQuestion] PRIMARY KEY CLUSTERED ([QuestionID] ASC),
    CONSTRAINT [FK_tblQuestion_tblCodeSet] FOREIGN KEY ([AnswerCodeSetFK]) REFERENCES [app].[tblCodeSet] ([CodeSetID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblQuestion]
    ON [app].[tblQuestion]([QuestionKey] ASC, [GroupTitle] ASC) WITH (FILLFACTOR = 90);

