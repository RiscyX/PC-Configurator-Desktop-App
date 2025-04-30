#!/bin/bash

# The file to import
DUMP_FILE="./dump/pcapp_dump.sql"

# MySQL container and database settings
MYSQL_CONTAINER="pcapp-db"
MYSQL_USER="root"
MYSQL_PASSWORD="root"
MYSQL_DATABASE="pcapp"

# Check if the file exists
if [ ! -f "$DUMP_FILE" ]; then
  echo "Error: File not found: $DUMP_FILE"
  exit 1
fi

# Import the database
docker exec -i $MYSQL_CONTAINER mysql -u$MYSQL_USER -p$MYSQL_PASSWORD $MYSQL_DATABASE < $DUMP_FILE

echo "Database import complete: $DUMP_FILE"
