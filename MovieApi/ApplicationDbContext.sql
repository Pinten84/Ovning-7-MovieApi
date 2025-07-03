IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE TABLE [Actors] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [BirthYear] int NOT NULL,
        CONSTRAINT [PK_Actors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE TABLE [Movies] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Year] int NOT NULL,
        [Genre] nvarchar(max) NOT NULL,
        [Duration] int NOT NULL,
        CONSTRAINT [PK_Movies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE TABLE [ActorMovie] (
        [ActorsId] int NOT NULL,
        [MoviesId] int NOT NULL,
        CONSTRAINT [PK_ActorMovie] PRIMARY KEY ([ActorsId], [MoviesId]),
        CONSTRAINT [FK_ActorMovie_Actors_ActorsId] FOREIGN KEY ([ActorsId]) REFERENCES [Actors] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ActorMovie_Movies_MoviesId] FOREIGN KEY ([MoviesId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE TABLE [MovieDetails] (
        [Id] int NOT NULL IDENTITY,
        [Synopsis] nvarchar(max) NOT NULL,
        [Language] nvarchar(max) NOT NULL,
        [Budget] decimal(18,2) NOT NULL,
        [MovieId] int NOT NULL,
        CONSTRAINT [PK_MovieDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovieDetails_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [ReviewerName] nvarchar(max) NOT NULL,
        [Comment] nvarchar(max) NOT NULL,
        [Rating] int NOT NULL,
        [MovieId] int NOT NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ActorMovie_MoviesId] ON [ActorMovie] ([MoviesId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MovieDetails_MovieId] ON [MovieDetails] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Reviews_MovieId] ON [Reviews] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250702093157_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250702093157_InitialCreate', N'9.0.6');
END;

COMMIT;
GO