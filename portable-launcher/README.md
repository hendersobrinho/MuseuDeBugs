# Atalho local portatil do MuseuDeBugs

Esta pasta guarda os scripts para rodar o MuseuDeBugs localmente em outra maquina Linux.

## Requisitos

- .NET SDK compativel com o projeto
- Node.js e npm
- PostgreSQL local configurado para o backend
- Connection string e credenciais locais do admin configuradas no backend

## Instalar o atalho

Na raiz do projeto:

```bash
chmod +x portable-launcher/install-shortcut.sh
./portable-launcher/install-shortcut.sh
```

Depois procure por `MuseuDeBugs` no menu de aplicativos e fixe na dock.

## Rodar sem instalar atalho

```bash
chmod +x portable-launcher/start-local.sh
./portable-launcher/start-local.sh
```

O script sobe:

- Backend: `http://localhost:5041`
- Frontend: `http://localhost:4200`

## O que mudar se precisar

Se mudar a porta do backend, ajuste tambem:

- `MuseuDeBugs.Api/Properties/launchSettings.json`
- `frontend/src/app/config/api.config.ts`

Se mudar a porta do frontend apenas para executar localmente:

```bash
MUSEUDEBUGS_FRONTEND_PORT=4300 ./portable-launcher/start-local.sh
```

Se quiser trocar o icone do atalho, edite `ICON_SOURCE` em:

```text
portable-launcher/install-shortcut.sh
```

O script copia este favicon transparente para a pasta local de icones e atualiza o atalho:

```text
frontend/public/favicon-512x512.png
```
