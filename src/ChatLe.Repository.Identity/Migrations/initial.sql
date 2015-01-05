CREATE TABLE [Attendees] (
    [ConversationId] nvarchar(128),
    [UserId] nvarchar(128),
    CONSTRAINT [PK_Attendees] PRIMARY KEY ([ConversationId], [UserId])
)
CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(128),
    [AccessFailedCount] int NOT NULL,
    [Email] nvarchar(max),
    [EmailConfirmed] bit NOT NULL,
    [LockoutEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NOT NULL,
    [NormalizedUserName] nvarchar(max),
    [PasswordHash] nvarchar(max),
    [PhoneNumber] nvarchar(max),
    [PhoneNumberConfirmed] bit NOT NULL,
    [SecurityStamp] nvarchar(max),
    [TwoFactorEnabled] bit NOT NULL,
    [UserName] nvarchar(max),
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
)
CREATE TABLE [Conversations] (
    [Id] nvarchar(128),
    CONSTRAINT [PK_Conversations] PRIMARY KEY ([Id])
)
CREATE TABLE [Messages] (
    [Id] nvarchar(128),
    [Date] datetime2 NOT NULL,
    [Text] nvarchar(max),
    [UserId] nvarchar(128),
    [ConversationId] nvarchar(128),
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id])
)
CREATE TABLE [NotificationConnections] (
    [ConnectionId] nvarchar(128),
    [NotificationType] nvarchar(128),
    [ConnectionDate] datetime2 NOT NULL,
    [UserId] nvarchar(128),
    CONSTRAINT [PK_NotificationConnections] PRIMARY KEY ([ConnectionId], [NotificationType])
)
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(128),
    [Name] nvarchar(max),
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
)
CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [RoleId] nvarchar(128),
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id])
)
CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [ClaimType] nvarchar(max),
    [ClaimValue] nvarchar(max),
    [UserId] nvarchar(128),
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id])
)
CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128),
    [ProviderKey] nvarchar(128),
    [ProviderDisplayName] nvarchar(max),
    [UserId] nvarchar(128),
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey])
)
CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(128),
    [RoleId] nvarchar(128),
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId])
)
ALTER TABLE [Attendees] ADD CONSTRAINT [FK_Attendees_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations] ([Id])
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations] ([Id])
ALTER TABLE [NotificationConnections] ADD CONSTRAINT [FK_NotificationConnections_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
ALTER TABLE [AspNetRoleClaims] ADD CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id])
ALTER TABLE [AspNetUserClaims] ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
ALTER TABLE [AspNetUserLogins] ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
INSERT INTO [__MigrationHistory] ([MigrationId], [ContextKey], [ProductVersion]) VALUES ('201412260841356_initial', 'ChatLe.Models.ChatLeIdentityDbContextSql', '7.0.0-beta1-11518')
