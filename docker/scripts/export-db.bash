#!/bin/bash

# File name and path
DUMP_FILE="./dump/pcapp_$(date +%Y%m%d%H%M%S).sql"

# MySQL container and database settings
MYSQL_CONTAINER="pcapp-db"
MYSQL_USER="root"
MYSQL_PASSWORD="root"
MYSQL_DATABASE="pcapp"

# Check if the dump folder exists, if not, create it
mkdir -p ./dump

# Export the database
docker exec -i $MYSQL_CONTAINER mysqldump -u$MYSQL_USER -p$MYSQL_PASSWORD $MYSQL_DATABASE > $DUMP_FILE

echo "Database export complete: $DUMP_FILE"
