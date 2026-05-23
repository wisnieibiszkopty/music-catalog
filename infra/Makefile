.PHONY: cluster deploy deploy-app deploy-monitoring setup destroy

cluster:
	minikube start --nodes 3 --cni calico
	kubectl wait --for=condition=Ready pods -l k8s-app=calico-node -n kube-system --timeout=120s
	minikube addons enable ingress
	minikube addons enable ingress-dns
	minikube addons enable storage-provisioner-rancher

deploy: cluster
	kubectl apply -f app/base/namespace.yaml
	kubectl apply -f monitoring/namespace.yaml
	helmfile sync -f app/helmfile.yaml
	helmfile sync -f monitoring/helmfile.yaml
	kubectl apply -k app/overlays/dev/
	kubectl apply -f monitoring/network-policies.yaml

deploy-app:
	kubectl apply -f app/base/namespace.yaml
	helmfile sync -f app/helmfile.yaml
	kubectl apply -k app/overlays/dev/

deploy-monitoring:
	kubectl apply -f monitoring/namespace.yaml
	helmfile sync -f monitoring/helmfile.yaml
	kubectl apply -f monitoring/network-policies.yaml

setup:
	find . -name "*.example" | while read f; do [ -f "$${f%.example}" ] || cp "$$f" "$${f%.example}"; done

destroy:
	minikube delete
