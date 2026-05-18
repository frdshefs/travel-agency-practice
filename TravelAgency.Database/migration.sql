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
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(50) NOT NULL,
    [ClientName] nvarchar(200) NOT NULL,
    [ClientEmail] nvarchar(200) NOT NULL,
    [ClientPhone] nvarchar(max) NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ConfirmedAt] datetime2 NULL,
    [SpecialRequests] nvarchar(max) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tours] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Destination] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    [Duration] int NOT NULL,
    [Hotel] nvarchar(max) NULL,
    [MealType] nvarchar(max) NULL,
    [IsAvailable] bit NOT NULL,
    [AvailableSeats] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Tours] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [TourId] int NOT NULL,
    [TourName] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    [PriceAtTime] decimal(18,2) NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO

CREATE INDEX [IX_OrderItems_TourId] ON [OrderItems] ([TourId]);
GO

CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
GO

CREATE INDEX [IX_Tours_Destination] ON [Tours] ([Destination]);
GO

CREATE INDEX [IX_Tours_Name] ON [Tours] ([Name]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518084633_InitialCreate', N'6.0.0');
GO

COMMIT;
GO

