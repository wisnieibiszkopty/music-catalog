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
