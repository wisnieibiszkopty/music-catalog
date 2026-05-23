# Zaawansowana konfiguracja Kubernetes

## Ograniczenie zasobów

Wszystkie kontenery mają zdefiniowane `requests` i `limits` dla CPU i pamięci. Keycloak jest wyjątkiem z limitem 1500Mi pamięci i requestem 512Mi, bo JVM przy starcie alokuje znacznie więcej niż w steady state. Pozostałe serwisy aplikacyjne mają stosunek request/limit 1:2 dla pamięci i 1:5 dla CPU.

## Polityki sieciowe

Oba namespace'y, music-catalog i monitoring, mają politykę `default-deny-all` blokującą cały ruch ingress i egress. Na tej bazie dodawane są wyłącznie niezbędne wyjątki.

`allow-dns-egress` jest zdefiniowane dla wszystkich podów i pozwala tylko na UDP/TCP port 53 do namespace kube-system. Bez tego żadne rozwiązywanie nazw DNS nie działa.

Każdy serwis ma osobną politykę ingress i egress. Na przykład artist-service przyjmuje ruch wyłącznie od api-gateway na port 8080, a wysyła ruch tylko do artist-db na port 5432, rabbitmq na port 5672 i keycloak na port 8080. Bazy danych akceptują połączenia wyłącznie od dedykowanego im serwisu aplikacyjnego.

scraper-service ma egress na port 443 do CIDR 0.0.0.0/0 z wyłączeniem prywatnych zakresów IP (10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16), bo musi trafiać do zewnętrznego API Spotify.

Polityka `allow-prometheus-scraping` w namespace music-catalog pozwala namespace monitoring na scraping metryk z portu 8080 wszystkich podów.

W namespace monitoring każdy komponent ma własne polityki. Grafana może łączyć się tylko z Prometheus na 9090 i Loki na 3100. Prometheus może scrapowaç kube-state-metrics, promtail, loki-canary oraz kube-apiserver. kube-state-metrics i promtail mają egress wyłącznie do kube-apiserver.

Takie podejście minimalizuje blast radius w razie kompromitacji jednego z komponentów i wymusza eksplicytne dokumentowanie dozwolonych ścieżek komunikacji.

## Planowanie rozmieszczenia podów

Serwisy używające wielu replik używają `podAntiAffinity` z `preferredDuringSchedulingIgnoredDuringExecution` i `topologyKey: kubernetes.io/hostname`. Dotyczy to: artist-service, catalog-service, api-gateway, web-app, orchestrator-service, notification-service, scraper-service. Każdy z nich ma 3 repliki.

Ma to na celu zapewnienie wdrożenia replik na różnych węzłach o ile to możliwe. 

Prometheus i Grafana w namespace monitoring używają `topologySpreadConstraints` z `maxSkew: 1`, `topologyKey: kubernetes.io/hostname` i `whenUnsatisfiable: ScheduleAnyway`. Scheduler stara się rozkładać pody równomiernie między węzłami.
