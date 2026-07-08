FROM mcr.microsoft.com/devcontainers/dotnet:10.0

# -----------------------------------------------------------------------------
# Base development image
#
# Keep this file focused on operating system level customization.
# Do NOT put project initialization here.
#
# Examples of future additions:
#   - apt packages
#   - OCR libraries
#   - native Linux dependencies
#   - enterprise CA certificates
# -----------------------------------------------------------------------------

RUN apt-get update \
    && apt-get install -y curl gpg \
    && curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /usr/share/keyrings/microsoft-prod.gpg \
    && echo "deb [signed-by=/usr/share/keyrings/microsoft-prod.gpg] https://packages.microsoft.com/debian/12/prod bookworm main" > /etc/apt/sources.list.d/microsoft-prod.list \
    && apt-get update \
    && apt-get install -y azure-functions-core-tools-4