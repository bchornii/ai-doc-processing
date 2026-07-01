#!/usr/bin/env bash
set -euo pipefail

echo "Validating development environment..."

# Ensure we are in repo root
REPO_ROOT="$(git rev-parse --show-toplevel)"
cd "$REPO_ROOT"

echo "Repository: $REPO_ROOT"

# Fix Windows + Linux container file mode noise
CURRENT_FILEMODE="$(git config --local --get core.filemode || true)"

if [ "$CURRENT_FILEMODE" != "false" ]; then
  echo "Setting repository-local core.filemode=false"
  git config --local core.filemode false
else
  echo "core.filemode already false"
fi

# Check for root-owned files inside Dev Container
if command -v id >/dev/null 2>&1 && [ "$(id -u)" -ne 0 ]; then
  ROOT_FILES="$(find . -user root \
    -not -path "./.git/*" \
    -not -path "./bin/*" \
    -not -path "./obj/*" \
    2>/dev/null | head -20 || true)"

  if [ -n "$ROOT_FILES" ]; then
    echo ""
    echo "WARNING: root-owned files found in repository:"
    echo "$ROOT_FILES"
    echo ""
    echo "Fix manually with:"
    echo "  sudo chown -R $(whoami):$(id -gn) ."
    echo ""
  else
    echo "No root-owned repository files found"
  fi
fi

# Warn about invalid Windows safe.directory entries inside Linux
if [ "$(uname -s)" = "Linux" ]; then
  INVALID_SAFE_DIRS="$(git config --global --get-all safe.directory 2>/dev/null | grep -E '^[A-Za-z]:' || true)"

  if [ -n "$INVALID_SAFE_DIRS" ]; then
    echo ""
    echo "WARNING: Windows-style safe.directory entries found in Linux Git config:"
    echo "$INVALID_SAFE_DIRS"
    echo ""
    echo "Recommended cleanup:"
    echo "  git config --global --unset-all safe.directory"
    echo "  git config --global --add safe.directory $REPO_ROOT"
    echo ""
  fi
fi

echo "Development environment validation complete."