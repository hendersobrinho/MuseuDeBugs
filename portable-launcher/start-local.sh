#!/usr/bin/env bash
set -Eeuo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FRONTEND_DIR="$ROOT_DIR/frontend"
API_PROJECT="$ROOT_DIR/MuseuDeBugs.Api/MuseuDeBugs.Api.csproj"
LOG_DIR="${XDG_STATE_HOME:-$HOME/.local/state}/museu-de-bugs/logs"

BACKEND_PORT="${MUSEUDEBUGS_BACKEND_PORT:-5041}"
FRONTEND_PORT="${MUSEUDEBUGS_FRONTEND_PORT:-4200}"
BACKEND_URL="http://localhost:${BACKEND_PORT}"
FRONTEND_URL="http://localhost:${FRONTEND_PORT}"

BACKEND_PID=""
FRONTEND_PID=""

mkdir -p "$LOG_DIR"

cleanup() {
  echo
  echo "Encerrando MuseuDeBugs local..."

  if [ -n "$FRONTEND_PID" ] && kill -0 "$FRONTEND_PID" >/dev/null 2>&1; then
    kill "$FRONTEND_PID" >/dev/null 2>&1 || true
  fi

  if [ -n "$BACKEND_PID" ] && kill -0 "$BACKEND_PID" >/dev/null 2>&1; then
    kill "$BACKEND_PID" >/dev/null 2>&1 || true
  fi
}

trap cleanup EXIT INT TERM

require_command() {
  local command_name="$1"
  local install_hint="$2"

  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Comando nao encontrado: $command_name" >&2
    echo "$install_hint" >&2
    exit 1
  fi
}

is_port_open() {
  local port="$1"

  if command -v ss >/dev/null 2>&1; then
    ss -ltn "( sport = :$port )" | tail -n +2 | grep -q .
    return
  fi

  if command -v lsof >/dev/null 2>&1; then
    lsof -iTCP:"$port" -sTCP:LISTEN -Pn >/dev/null 2>&1
    return
  fi

  curl -fsS "http://localhost:$port" >/dev/null 2>&1
}

wait_for_url() {
  local url="$1"
  local name="$2"
  local attempts="${3:-90}"

  for _ in $(seq 1 "$attempts"); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      return 0
    fi
    sleep 1
  done

  echo "Nao consegui confirmar que $name iniciou em $url." >&2
  return 1
}

ensure_backend() {
  if is_port_open "$BACKEND_PORT"; then
    echo "Backend ja esta rodando em $BACKEND_URL."
    return
  fi

  echo "Iniciando backend em $BACKEND_URL..."
  dotnet run --project "$API_PROJECT" --launch-profile http \
    >"$LOG_DIR/backend.log" 2>&1 &
  BACKEND_PID="$!"
}

ensure_frontend_dependencies() {
  if [ -d "$FRONTEND_DIR/node_modules" ]; then
    return
  fi

  echo "Instalando dependencias do frontend..."
  (
    cd "$FRONTEND_DIR"
    npm install
  ) >"$LOG_DIR/frontend-install.log" 2>&1
}

ensure_frontend() {
  if is_port_open "$FRONTEND_PORT"; then
    echo "Frontend ja esta rodando em $FRONTEND_URL."
    return
  fi

  ensure_frontend_dependencies

  echo "Iniciando frontend em $FRONTEND_URL..."
  (
    cd "$FRONTEND_DIR"
    npm start -- --host 127.0.0.1 --port "$FRONTEND_PORT"
  ) >"$LOG_DIR/frontend.log" 2>&1 &
  FRONTEND_PID="$!"
}

open_browser() {
  if command -v xdg-open >/dev/null 2>&1; then
    xdg-open "$FRONTEND_URL" >/dev/null 2>&1 &
    return
  fi

  echo "Abra manualmente: $FRONTEND_URL"
}

require_command "dotnet" "Instale o .NET SDK compativel com o projeto."
require_command "npm" "Instale Node.js e npm."
require_command "curl" "Instale curl para o script conseguir esperar os servidores subirem."

ensure_backend
ensure_frontend
wait_for_url "$BACKEND_URL/swagger/index.html" "backend" 90 || true
wait_for_url "$FRONTEND_URL" "frontend" 90
open_browser

echo "MuseuDeBugs local iniciado."
echo "Frontend: $FRONTEND_URL"
echo "Backend:  $BACKEND_URL"
echo "Logs:     $LOG_DIR"
echo
echo "Mantenha esta janela aberta enquanto estiver usando o app."
echo "Feche a janela ou pressione Ctrl+C para parar o frontend e o backend."

wait
