# Use the official PostgreSQL image from the Docker Hub
FROM postgres:13

# Environment variables to configure the database
ENV POSTGRES_DB=localizadb
ENV POSTGRES_USER=localizador
ENV POSTGRES_PASSWORD=localizapw

# Copy the SQL script to the Docker container
COPY init.sql /docker-entrypoint-initdb.d/

# The default command will run the PostgreSQL server
