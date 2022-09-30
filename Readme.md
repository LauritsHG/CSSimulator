# CSSimulator
*Central backend System*

CSSimulator er en Mock for et centralt system til at kommunikere med ladestandere. Hertil bruges systemetet LFA (https://github.com/filip8600/LFA), som håndtere de mange Websocket-forbindelser fra ladestandere.
CS og LFA er designet med et actor system "Proto.Actor", for kommunikation imellem sig.

Systemkrav:
- Docker (Herunder Kubectl)
- MiniKube (Lokalt Kubernetes Cluster)
- Helm (Installering af images til Cluster)

Opsætning og kørsel:
- ```git clone https://github.com/LauritsHG/CSSimulator```
- Byg og Push Image (Fra mappe med Dockerfile) ```docker build -t filip8600/CSSimulator:latest```
- Push til Image Repository (Fx Docker Hub) ```docker push filip8600/CSSimulator:latest```
- Start Kluster ```./minicube.exe start```
- Release image til Cluster: ```helm install CSSimulator chart-helm```
- Verificer at pods kører: ```Kubectl get pods```
