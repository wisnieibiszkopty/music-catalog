## namespace

Wszystkie zasoby aplikacyjne wdrożone w namespace `music-catalog`, stack monitoringowy w osobnym namespace `monitoring` w celu zapewnienia izolacji od właściwej aplikacji.

---

## deployment

api gateway - 3 repliki, bezstanowy serwis, przechodzi przez niego cały ruch

web-app - 3 repliki, statyczny serwer dla aplikacji webowej, brak stanu

artists-service, catalog-service, notification-service, scraper-service - bezstanowe serwisy udostępniające interfejsy REST API. 

Dodatkowo artists-service i catalog-service używają initContainers do migracji baz danych.

### statefulSet

artist-db, catalog-db, keycloak-db - pojedyncza replika, aktualnie bez skalowania. Przechowują stan (baza danych) dlatego używają stateful set.


## services

Wszystkie serwisy aplikacyjne (api-gateway, web-app, artist-service, catalog-service, notification-service, orchestrator-service, scraper-service, keycloak) używają ClusterIP — są dostępne wyłącznie wewnątrz klastra, dostęp zewnętrzny realizowany jest przez Ingress, więc NodePort ani LoadBalancer nie są potrzebne.

Serwisy baz danych (artist-db, catalog-db, keycloak-db) używają headless ClusterIP `clusterIP: None` — StatefulSet wymaga stabilnego DNS per pod `pod-0.service-name`; ClusterIP z load balancingiem przy pojedynczej replice bazy nie ma sensu.


## ingress

Użyto nginx Ingress Controller — cały ruch zewnętrzny wchodzi przez jeden punkt na hoście music-catalog.local i trafia do api-gateway. Ruch jest szyfrowany TLS z self-signed certyfikatem zarządzanym przez cert-manager. Grafana dostępna pod /grafana.

## PV / PVC / storageClass

Do przechowywania danych użyto StorageClass local-path z Rancher local-path provisioner. 
Provisioner dynamicznie tworzy PV na węźle, na którym zaplanowany jest pod.

Trwały stan trzymają trzy bazy PostgreSQL — artist-db, catalog-db, keycloak-db — 
każda jako StatefulSet z volumeClaimTemplates. Kubernetes tworzy PVC automatycznie 
dla poda i zachowuje go między restartami. Redis i RabbitMQ zarządzają własnym 
storage wewnętrznie przez Helm charty. Stack monitoringowy — Prometheus, Grafana, 
Loki — również korzysta z osobnych PVC.

### secrets

Sekrety generowane są przez Kustomize secretGenerator z lokalnych plików .env.
Wstrzykiwane do kontenerów jako zmienne środowiskowe przez secretKeyRef — każdy
kontener dostaje tylko konkretne klucze, nie cały Secret.

### configMaps

Konfiguracja z ConfigMap trafia do kontenera jako zmienne środowiskowe —
przez envFrom dla całego ConfigMap lub configMapKeyRef dla wybranych kluczy.
Realm Keycloak montowany jako plik przez volumeMount.
