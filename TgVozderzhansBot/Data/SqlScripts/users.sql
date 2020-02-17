CREATE TABLE IF NOT EXISTS Users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username VARCHAR(255) NOT NULL,
    ChatId INTEGER NOT NULL,
    CreatedAt DATETIME,
    AllowAddDays INTEGER default 1,
    Status INTEGER default 0 not null
)