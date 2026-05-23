W celu uruchomienia opracowanego systemu można skorzystać z pliku Makefile znajdującego się w folderze /infra. Do działania system potrzebuje 3 węzłów oraz Calico. Można do tego użyć polecenia: 

```
minikube start --nodes 3 --cni calico
```

Po starcie aplikacji wymagane dodane addonów:

```
minikube addons enable ingress
minikube addons enable ingress-dns
minikube addons enable storage-provisioner-rancher
```

Następnie należy kolejno zaaplikować plik yaml /app/base/namespace.yaml, oraz /monitoring/namespace.yaml. Po nich użyć polecenia helmfile sync dla obu plików helmfile.yaml, a następne użyć:

```
kubectl apply -k app/overlays/dev/
kubectl apply -f monitoring/network-policies.yaml
```

