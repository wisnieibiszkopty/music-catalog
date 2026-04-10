## flyway/flyway:12.0-alpine

Obraz ten jest używany przy migracjach baz danych działających w systemie. Nie jest on
wstanie komunikować się ze światem zewnętrznym oraz zatrzymuje się po wykonaniu migracji.
Ze względu na jego naturę nie wykorzystuje on ssl/tls do komunikacji z światem zewnętrznym.
Wobec tego podatności związane z ssl nie stanowią zagrożenia. 

Podatności związane z netty i jackson mogłyby zostać wykorzystane gdyby do usługi był 
kierowany ruch z zewnątrz - nie dotyczy.

Podatności związane z libpng i zlib też nie stanowią zagrożenia gdyż te biblioteki nie są 
wykorzystywane. 

CVE-2025-59250 - podatność związana z sqlserver, nie dotyczy w projekcie używany jest postgres.

## grafana/grafana:12.4

CVE-2026-28390 - dotyvzy nieużywanego w aplikacji protokołu

CVE-2026-34986, CVE-2026-34040 - nietypowe funkcje prawdpoodbnie nieużywane w aplikacji

CVE-2026-24051, CVE-2026-39883 - nie dotyczy systemu na którym działa aplikacja

## grafana/loki:3.7

jak w przypadku grafany

## grafana/promtail:3.6

jak w przypadku grafany

## bin/prometheus

jak w przypadku grafany

## quay.io/keycloak/keycloak:26.6

Kolejne błędy z libpng nie dotyczącym aplikacji. Błąd związany z sql-server,
jak wyżej nie dotyczy. 

## rabbitmq:4.2-alpine

zlib nieuzywany, komunikacja CMS nie dotyczy

## redis:8.6-alpine

jak wyżej 

## wisnieibiszkopty/music-catalog-api-gateway:0.1.0

jak wyżej

## wisnieibiszkopty/music-catalog-web-app:0.1.0

jak wyżej