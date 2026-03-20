CREATE TABLE "Albums" (
  "Id" TEXT PRIMARY KEY,
  "Name" TEXT NOT NULL,
  "ArtistId" TEXT NOT NULL,
  "ReleaseDate" TIMESTAMP NOT NULL,
  "TotalTracks" INT NOT NULL,
  "ImageUrl" TEXT
);

CREATE TABLE "Tracks" (
  "Id" SERIAL PRIMARY KEY,
  "AlbumId" TEXT NOT NULL,
  "Name" TEXT NOT NULL,
  "DurationMs" INT NOT NULL,
  "TrackNumber" INT NOT NULL,

  CONSTRAINT "FK_Tracks_Albums_AlbumId"
      FOREIGN KEY ("AlbumId")
          REFERENCES "Albums"("Id")
          ON DELETE CASCADE
);

CREATE INDEX "IX_Tracks_AlbumId" ON "Tracks"("AlbumId");