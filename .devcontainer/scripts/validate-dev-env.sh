#!/usr/bin/env bash

set -euo pipefail

echo "========================================="
echo "Validating development environment..."
echo "========================================="

# Ensure we are in repository root
REPO_ROOT="$(git rev-parse --show-toplevel)"
cd "$REPO_ROOT"

echo "Repository: $REPO_ROOT"
echo ""

###############################################################################
# Git configuration
###############################################################################

CURRENT_FILEMODE="$(git config --local --get core.filemode || true)"

if [ "$CURRENT_FILEMODE" != "false" ]; then
    echo "Configuring repository-local Git settings..."
    git config --local core.filemode false
else
    echo "Git configuration OK"
fi

###############################################################################
# Root-owned files
###############################################################################

ROOT_FILES="$(
find . \
    -user root \
    -not -path "./.git/*" \
    -not -path "*/bin/*" \
    -not -path "*/obj/*" \
    2>/dev/null | head -20 || true
)"

if [ -n "$ROOT_FILES" ]; then
    echo ""
    echo "ERROR: Root-owned files detected."
    echo ""
    echo "$ROOT_FILES"
    echo ""
    echo "This usually happens when:"
    echo "  • an earlier Dev Container was running as root"
    echo "  • build commands were executed with sudo"
    echo ""
    echo "Recommended recovery:"
    echo ""
    echo "  sudo chown -R $(whoami):$(id -gn) ."
    echo "  ./scripts/clean.sh"
    echo ""

    exit 1
fi

echo "Repository ownership OK"

###############################################################################
# Git safe.directory validation
###############################################################################

if [ "$(uname -s)" = "Linux" ]; then

    INVALID_SAFE_DIRS="$(
        git config --global --get-all safe.directory 2>/dev/null \
        | grep -E '^[A-Za-z]:' || true
    )"

    if [ -n "$INVALID_SAFE_DIRS" ]; then

        echo ""
        echo "WARNING: Windows-style safe.directory entries detected:"
        echo ""
        echo "$INVALID_SAFE_DIRS"
        echo ""
        echo "Recommended cleanup:"
        echo ""
        echo "  git config --global --unset-all safe.directory"
        echo "  git config --global --add safe.directory $REPO_ROOT"
        echo ""

    else
        echo "Git safe.directory configuration OK"
    fi
fi

###############################################################################
# Azure Functions Worker SDK validation
###############################################################################

WORKER_EXTENSIONS="$(
find . \
    -path "*/WorkerExtensions/WorkerExtensions.csproj" \
    2>/dev/null || true
)"

if [ -n "$WORKER_EXTENSIONS" ]; then

    ROOT_WORKER="$(
        find . \
            -path "*/WorkerExtensions/WorkerExtensions.csproj" \
            -user root \
            2>/dev/null || true
    )"

    if [ -n "$ROOT_WORKER" ]; then

        echo ""
        echo "ERROR: Azure Functions WorkerExtensions contains root-owned files."
        echo ""
        echo "$ROOT_WORKER"
        echo ""
        echo "Delete generated artifacts:"
        echo ""
        echo "  ./scripts/clean.sh"
        echo ""
        echo "then rebuild."
        echo ""

        exit 1
    fi
fi

echo "Azure Functions WorkerExtensions OK"

echo ""
echo "Development environment validation completed successfully."