CREATE TABLE albums (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    artist_id TEXT NOT NULL,
    release_date TEXT NOT NULL,
    total_tracks INT NOT NULL,
    image_url TEXT
);

CREATE TABLE tracks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    album_id TEXT NOT NULL,
    name TEXT NOT NULL,
    duration_ms INT NOT NULL,
    track_number INT NOT NULL,

    CONSTRAINT fk_tracks_album
        FOREIGN KEY (album_id)
            REFERENCES albums(id)
            ON DELETE CASCADE
);

CREATE INDEX ix_tracks_album_id ON tracks(album_id);