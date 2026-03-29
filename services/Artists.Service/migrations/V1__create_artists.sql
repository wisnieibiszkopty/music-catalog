CREATE TABLE artists (
    id VARCHAR(22) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    founded_year INT NULL,
    description TEXT NULL,
    image_url TEXT NULL,
    is_band BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE genres (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE artist_genres (
    artist_id VARCHAR(22) REFERENCES artists(id) ON DELETE CASCADE,
    genre_id INT REFERENCES genres(id) ON DELETE CASCADE,
    PRIMARY KEY (artist_id, genre_id)
);


CREATE INDEX idx_artist_genres_genre_id ON artist_genres(genre_id);