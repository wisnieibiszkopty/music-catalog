# music-catalog

```
music-catalog/
├── docker/
│   └── docker-compose.yaml
├── docs/
├── frontend/
│   └── Dockerfile
├── infra/
├── services/
│   ├── ApiGateway/
│   │   └── Dockerfile
│   ├── Artists.Service/
│   │   └── Dockerfile
│   ├── Catalog.Service/
│   │   └── Dockerfile
│   ├── Notification.Service/
│   │   └── Dockerfile
│   ├── Orchestrator.Service/
│   │   └── Dockerfile
│   └── Scraper.Service/
│       └── Dockerfile
├── shared/
├── .dockerignore
├── .gitignore
├── MusicCatalog.slnx
└── README.md
```

## Architecture overview

![Architecture overview](docs/architecture.svg)

## Artist scraper event flow

![Architecture overview](docs/artist-scraper-flow.png)

## Albums scraper event flow

![Architecture overview](docs/albums-scraper-flow.png)

## Docker compose schema

![Docker compose schema](docs/docker-compse-schema.svg)
