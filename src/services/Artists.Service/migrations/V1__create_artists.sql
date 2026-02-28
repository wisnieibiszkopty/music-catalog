CREATE TABLE artists (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
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
    artist_id UUID REFERENCES artists(id) ON DELETE CASCADE,
    genre_id INT REFERENCES genres(id) ON DELETE CASCADE,
    PRIMARY KEY (artist_id, genre_id)
);

CREATE TABLE musicians (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    bio TEXT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE artist_musicians (
    artist_id UUID REFERENCES artists(id) ON DELETE CASCADE,
    musician_id UUID REFERENCES musicians(id) ON DELETE CASCADE,
    role VARCHAR(100) NULL,
    PRIMARY KEY (artist_id, musician_id)
);

CREATE INDEX idx_artist_genres_genre_id ON artist_genres(genre_id);
CREATE INDEX idx_artist_musicians_musician_id ON artist_musicians(musician_id);