﻿CREATE TABLE IF NOT EXISTS Friends (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    FriendId INTEGER NOT NULL,
    CreatedAt DATETIME NOT NULL
)