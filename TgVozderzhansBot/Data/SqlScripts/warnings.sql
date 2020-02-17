CREATE TABLE IF NOT EXISTS Warnings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    AbsItemId INTEGER NOT NULL,
    ActiveUntil DATETIME NOT NULL,
    IsNotified INTEGER default 0,
    IsConfirmed INTEGER default 0,
    CreatedAt DATETIME NOT NULL
)