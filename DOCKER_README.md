# Docker Setup Guide - Tarification Backend

## 📋 Prérequis

- Docker et Docker Compose installés
- Port 8080 (API) et 5432 (PostgreSQL) disponibles

## 🚀 Démarrage rapide

### Production (docker-compose.yml)
```bash
# Construire et démarrer les services
docker-compose up -d

# Vérifier le statut
docker-compose ps

# Consulter les logs
docker-compose logs -f api
docker-compose logs -f postgres
```

### Développement (docker-compose.dev.yml)
```bash
# Démarrer avec la configuration dev
docker-compose -f docker-compose.dev.yml up -d
```

## 📝 Commandes utiles

### Arrêter les services
```bash
docker-compose down
```

### Arrêter et supprimer les volumes (nettoyage complet)
```bash
docker-compose down -v
```

### Afficher les logs
```bash
# Tous les logs
docker-compose logs

# Logs en direct
docker-compose logs -f

# Logs d'un service spécifique
docker-compose logs -f api
docker-compose logs -f postgres
```

### Accéder au shell du conteneur
```bash
# API
docker exec -it tarification_api /bin/sh

# PostgreSQL
docker exec -it tarification_db psql -U postgres -d TarificationFacturation_BDF
```

### Construire l'image manuellement
```bash
docker build -t tarification-api:latest .
```

### Exécuter l'image seule
```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=<db_host>;Port=5432;Database=TarificationFacturation_BDF;Username=postgres;Password=0000" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  tarification-api:latest
```

## 🔧 Configuration

### Variables d'environnement
Les fichiers `docker-compose.yml` et `docker-compose.dev.yml` contiennent les variables d'environnement essentielles. Vous pouvez les adapter selon vos besoins.

### Secrets sensibles
⚠️ **Important:** Les secrets (clés JWT, mots de passe SMTP, etc.) sont actuellement dans les fichiers docker-compose. Pour la production, utilisez:
- Docker Secrets
- Variables d'environnement sécurisées
- Gestionnaire de secrets (HashiCorp Vault, Azure Key Vault, etc.)

## 📊 Structure

- **Dockerfile**: Build multi-étapes optimisé pour .NET 8.0
- **docker-compose.yml**: Configuration production avec PostgreSQL
- **docker-compose.dev.yml**: Configuration développement
- **.dockerignore**: Fichiers exclus du contexte Docker

## ✅ Vérification du fonctionnement

Une fois démarré, l'API devrait être accessible sur:
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger` (si configuré)

### Health Check
```bash
curl -f http://localhost:8080/health
```

## 🐛 Dépannage

### Les conteneurs ne démarrent pas
```bash
docker-compose logs -f
```

### Permission denied (Linux/Mac)
```bash
sudo docker-compose up -d
```

### Port déjà utilisé
Modifier les ports dans `docker-compose.yml`:
```yaml
ports:
  - "8081:8080"  # Accès externe sur 8081
```

### Problèmes de connexion BD
- Vérifier que PostgreSQL est en bon état: `docker-compose ps`
- Vérifier les logs de la BD: `docker-compose logs postgres`
- Vérifier la chaîne de connexion dans les variables d'environnement

## 📦 Déploiement

### Registry Docker
```bash
# Tagger l'image
docker tag tarification-api:latest your-registry/tarification-api:latest

# Pousser vers le registry
docker push your-registry/tarification-api:latest
```

## 📚 Documentation supplémentaire

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET 8 Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
