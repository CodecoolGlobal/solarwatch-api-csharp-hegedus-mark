#!/bin/sh
# entrypoint.sh

# Print a message to verify the script is running
echo "Running database migrations..."

# Start the application
dotnet SolarWatch.dll