#!/usr/bin/env bash

set -euo pipefail

echo "Removing bin directories..."
find . -name bin -type d -exec rm -rf {} +

echo "Removing obj directories..."
find . -name obj -type d -exec rm -rf {} +

echo "Build artifacts removed."