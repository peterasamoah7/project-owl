CREATE TABLE [Audios] (
    [Id] int NOT NULL IDENTITY,
    [FileName] nvarchar(max) NULL,
    [FileExtension] nvarchar(max) NULL,
    [Sentiment] decimal(18,2) NOT NULL,
    [Taxonomy] nvarchar(max) NULL,
    [Issue] int NOT NULL,
    [Transcript] nvarchar(max) NULL,
    [Created] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Audios] PRIMARY KEY ([Id])
);

GO