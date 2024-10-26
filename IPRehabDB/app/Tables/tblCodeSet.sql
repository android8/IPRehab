CREATE TABLE [app].[tblCodeSet] (
    [CodeSetID]       INT           IDENTITY (1, 1) NOT NULL,
    [CodeSetParent]   INT           NULL,
    [CodeValue]       VARCHAR (30)  NOT NULL,
    [CodeDescription] VARCHAR (400) NOT NULL,
    [HierarchyType]   INT           NULL,
    [Active]          BIT           NULL,
    [FyConstraint]    INT           NULL,
    [SortOrder]       INT           NULL,
    [Comment]         VARCHAR (200) NULL,
    [LastUpdate]      DATETIME      NOT NULL,
    CONSTRAINT [PK_tblCodeSet] PRIMARY KEY CLUSTERED ([CodeSetID] ASC),
    CONSTRAINT [FK_tblCodeSet_tblCodeSet] FOREIGN KEY ([CodeSetParent]) REFERENCES [app].[tblCodeSet] ([CodeSetID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblCodeSet]
    ON [app].[tblCodeSet]([CodeSetParent] ASC, [CodeValue] ASC);

